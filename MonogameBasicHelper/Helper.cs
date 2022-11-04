using MonogameBasicHelperDLL.ContainerService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using MonogameBasicHelperDLL.Events;
using System.Drawing;
using System.Windows.Forms;

namespace MonogameBasicHelperDLL
{
    internal static class Helper
    {
        private static string InterfaceName { get; set; } = typeof(INotificationReceiver<dynamic>).Name;
        public static IEnumerable<Type> GetNotificationReceivers(object instance)
        {
            Type type = instance.GetType();
            foreach (var irfc in type.GetInterfaces())
            {
                string name = irfc.Name;
                if (name.Equals(InterfaceName))
                    yield return irfc;
            }
        }

        public static object GetLateInit(Type genericType, dynamic adapter)
        {
            var type = typeof(LateInit<>).MakeGenericType(genericType);
            dynamic instance = Activator.CreateInstance(type);
            instance.Value = adapter;

            return instance;
        }

        public static IEnumerable<MethodInfo> GetMethodsWithAttribute(Type type, Type attributeType)
        {
            var currentMethods = type.GetMethods();
            foreach (var method in currentMethods)
            {
                if (method.CustomAttributes.Any(p => p.AttributeType.Equals(attributeType)))
                {
                    yield return method;
                }
            }
        }

        public static IEnumerable<PropertyInfo> GetPropertiesWithAttribute(Type type, Type attributeType)
        {
            var currentMethods = type.GetProperties();
            foreach (var method in currentMethods)
            {
                if (method.CustomAttributes.Any(p => p.AttributeType.Equals(attributeType)))
                {
                    yield return method;
                }
            }
        }

        public static SizeF MeasureStringSize(Label label)
        {
            Graphics gfx = Graphics.FromImage(new Bitmap(1, 1));
            return gfx.MeasureString(label.Text, label.Font);
        }

    }
}