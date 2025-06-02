using System;
using System.Collections.Generic;

namespace MyUtilities
{
    public class ServiceLocator
    {
        private static Dictionary<Type, object> services = new Dictionary<Type, object>();

        public static void Register<T>(T service) where T : class
        {
            if (!services.ContainsKey(typeof(T)))
            {
                services.Add(typeof(T), service);
            }
        }
    
        public static void Unregister<T>(T service) where T : class
        {
            if (services.TryGetValue(typeof(T), out var registeredService) && registeredService.Equals(service))
            {
                services.Remove(typeof(T));
            }
        }
    
        public static T Get<T>() where T : class
        {
            return services[typeof(T)] as T;
        }
    }
}
