using System;
using System.Collections.Generic;

namespace MonogameBasicHelper.Events
{
    internal class FlexibleEventHandler
    {
        private static FlexibleEventHandler instance;
        public static FlexibleEventHandler Default()
        {
            if (instance == null)
                instance = new FlexibleEventHandler();
            return instance;
        }
        private Dictionary<Type, List<object>> typeInstances;
        private FlexibleEventHandler()
        {
            typeInstances = new Dictionary<Type, List<object>>();
        }
        public void Subscribe(object instance)
        {
            foreach (var irfc in Helper.GetNotificationReceivers(instance))
            {
                if (!typeInstances.ContainsKey(irfc))
                    typeInstances.Add(irfc, new List<object>());
                typeInstances[irfc].Add(instance);
            }
        }
        public void Unsubscribe(object instance)
        {
            typeInstances[instance.GetType()].Remove(instance);
        }
        public IEnumerable<object> GetRelevantInstances<S>()
        {
            if (!typeInstances.ContainsKey(typeof(S)))
                yield break;

            var instances = typeInstances[typeof(S)];
            for (int i = 0; i < instances.Count; i++)
                yield return instances[i];
        }
    }
}
