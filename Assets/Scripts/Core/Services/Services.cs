using System;
using System.Collections.Generic;
using UnityEngine;

namespace PocketDungeons.Core.Services
{
    /// <summary>
    /// Service Locator — register services during Boot, access anywhere via Services.Get&lt;T&gt;().
    /// All services are coded to interfaces for testability and decoupling.
    /// </summary>
    public static class Services
    {
        private static readonly Dictionary<Type, object> Registry = new();

        public static void Register<T>(T service) where T : class
        {
            var type = typeof(T);
            if (Registry.ContainsKey(type))
            {
                Debug.LogWarning($"[Services] Overwriting existing service: {type.Name}");
            }

            Registry[type] = service;
            Debug.Log($"[Services] Registered: {type.Name}");
        }

        public static T Get<T>() where T : class
        {
            var type = typeof(T);
            if (Registry.TryGetValue(type, out var service))
            {
                return (T)service;
            }

            Debug.LogError($"[Services] Service not found: {type.Name}. Was it registered during Boot?");
            return null;
        }

        public static bool TryGet<T>(out T service) where T : class
        {
            var type = typeof(T);
            if (Registry.TryGetValue(type, out var obj))
            {
                service = (T)obj;
                return true;
            }

            service = null;
            return false;
        }

        public static void Unregister<T>() where T : class
        {
            var type = typeof(T);
            if (Registry.Remove(type))
            {
                Debug.Log($"[Services] Unregistered: {type.Name}");
            }
        }

        /// <summary>
        /// Clear all services. Call only during application quit or test teardown.
        /// </summary>
        public static void Clear()
        {
            Registry.Clear();
            Debug.Log("[Services] All services cleared.");
        }
    }
}
