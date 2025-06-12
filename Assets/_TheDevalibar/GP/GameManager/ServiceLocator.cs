using System;
using System.Collections.Generic;
using UnityEngine;

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
            object service = services[typeof(T)];
            if (service == null) return default;
            T t = service as T;
            return t;
        }
        
        public static void RequireService<T>(Component context, ref T field, string errorMessage) where T : class
        {
            if (field == null)
            {
                field = Get<T>();
                if (field == null)
                {
                    Debug.LogError(errorMessage, context);
                }
            }
        }
        
        public static void RequireComponent<T>(Component context, ref T field, string errorMessage) where T : Component
        {
            if (field == null && context != null)
            {
                field = context.GetComponent<T>();
                if (field == null)
                {
                    Debug.LogError(errorMessage, context);
                }
            }
        }
    }
}
