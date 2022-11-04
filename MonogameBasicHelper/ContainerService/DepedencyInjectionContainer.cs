using System;
using System.Collections.Generic;
using System.Linq;
using MonogameBasicHelper.Attributes;
using MonogameBasicHelperDLL.Events;
using MonogameBasicHelperDLL.MVVM;
namespace MonogameBasicHelperDLL.ContainerService
{
    public class MonogameDepdencyInjection
    {
        internal struct T_Info
        {
            internal Type Type;

            public override int GetHashCode()
            {
                return Type.Name.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return obj.GetHashCode().Equals(this.GetHashCode());
            }
        }

        private Dictionary<T_Info, Lazy<IViewModel>> _viewModelList = new Dictionary<T_Info, Lazy<IViewModel>>();
        private Dictionary<T_Info, Lazy<IAdapter>> _adapterList = new Dictionary<T_Info, Lazy<IAdapter>>();
        private Dictionary<T_Info, dynamic> _earlyInits = new Dictionary<T_Info, dynamic>();

        private static MonogameDepdencyInjection _depedencyInjectionContainerInstance;
        public static MonogameDepdencyInjection Services
        {
            get
            {
                if (_depedencyInjectionContainerInstance == null)
                    _depedencyInjectionContainerInstance = new MonogameDepdencyInjection();
                return _depedencyInjectionContainerInstance;
            }
        }

        private MonogameDepdencyInjection()
        {

        }

        /// <summary>
        /// Manages classes that implement IViewModel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        public void AddViewModel<T>(params object[] args) where T : class, IViewModel
        {
            _viewModelList.Add(new T_Info()
            {
                Type = typeof(T)
            },
            new Lazy<IViewModel>(() =>
            {
                return GetInstance<T>(args);
            }));
        }

        /// <summary>
        /// Manages classes that implement IAdapter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        public void AddAdapter<T>(params object[] args) where T : class, IAdapter
        {
            _adapterList.Add(new T_Info()
            {
                Type = typeof(T)
            },
            new Lazy<IAdapter>(() =>
            {
                return GetInstance<T>(args);
            }));
        }

        /// <summary>
        /// Returns a viewmodel that was added previously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetViewModel<T>() where T : class, IViewModel
        {
            return (T)_viewModelList[new T_Info()
            {
                Type = typeof(T)
            }].Value;
        }

        private T_Info GetSuitableAdapter(Type forInterface)
        {
            foreach (var adapter in _adapterList)
            {
                var interfaces = adapter.Key.Type.GetInterfaces();
                if (interfaces.Contains(forInterface))
                {
                    return adapter.Key;
                }

            }

            throw new Exception("Kein Adapter gefunden.");
        }


        /// <summary>
        /// Returns an instance of the specified type. The object is completely managed by the library.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        public T New<T>(params object[] args)
        {
            return GetInstance<T>(args);
        }

        private T GetInstance<T>(params object[] args)
        {
            var currentInspectedType = typeof(T);
            var ctors = currentInspectedType.GetConstructors();
            if (ctors.Length > 1)
                throw new InvalidOperationException("Die vom Depedency Injection verwaltete Klasse darf nicht über mehr als ein Konstruktor verfügen.");

            T currentInstance = default(T);

            var ctor = ctors[0];
            List<object> suitableDependencies = new List<object>();
            List<dynamic> lateInitAdapters = new List<dynamic>();

            foreach (var parameters in ctor.GetParameters())
            {
                var interfaces = parameters.ParameterType.GetInterfaces();
                if (interfaces.Contains(typeof(IAdapter)))
                {
                    var adapterInstance = GetAdapter(parameters.ParameterType);
                    if (adapterInstance != null)
                        suitableDependencies.Add(adapterInstance);
                    else if (parameters.ParameterType.IsInterface)
                        suitableDependencies.Add(_adapterList[GetSuitableAdapter(parameters.ParameterType)].Value);
                }
                else if (parameters.ParameterType.Name.Contains(typeof(LateInit<>).Name))
                {
                    var genericType = parameters.ParameterType.GetGenericArguments()[0];
                    var instance = _adapterList.FirstOrDefault(p => p.Key.Equals(genericType.Name)).Value;
                    var lateInit = Helper.GetLateInit(genericType, instance.Value);

                    suitableDependencies.Add(lateInit);
                    lateInitAdapters.Add(lateInit);
                }
                else if (interfaces.Contains(typeof(IViewModel)))
                {
                    throw new InvalidOperationException("View Model über GetViewModel erzeugen'.");
                }
            }

            currentInstance = (T)Activator.CreateInstance(currentInspectedType, suitableDependencies.ToArray());

            var earlyInit = _earlyInits.FirstOrDefault(p => p.Key.Type.Equals(currentInspectedType));
            if (earlyInit.Key.Type != null)
            {
                earlyInit.Value.Invoke(currentInstance);
                _earlyInits.Remove(earlyInit.Key);
            }

            foreach (var lateInit in lateInitAdapters)
                lateInit.Activated = true;

            AddSubscription(currentInspectedType, currentInstance);
            var appropiateMethods = Helper.GetMethodsWithAttribute(currentInspectedType, typeof(OnBuiltUp));

            if (appropiateMethods.Any())
                appropiateMethods.First().Invoke(currentInstance, args);

            return currentInstance;
        }

        private IAdapter GetAdapter(Type type)
        {
            return _adapterList.FirstOrDefault(p => p.Key.Equals(type.Name)).Value?.Value;
        }

        private void AddSubscription(Type type, object instance)
        {
            var inspectedInterfaces = type.GetInterfaces();
            if (inspectedInterfaces.ToList().Exists(p => p.Name.Contains(typeof(INotificationReceiver<dynamic>).Name)))
                FlexibleEventHandler.Default().Subscribe(instance);
        }

        /// <summary>
        /// Use this method to initialize a view model before adapters are loaded.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        public void AddPreStartInitialization<T>(Action<T> action) where T : IViewModel
        {
            _earlyInits.Add(new T_Info() { Type = typeof(T) }, action);
        }
    }
}