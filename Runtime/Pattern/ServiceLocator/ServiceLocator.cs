using System;
using System.Collections.Generic;
using Base.Core;
using Base.Helper;
using Base.Logging;
using Base.Module;
using UnityEngine;

namespace Base.Pattern
{
    /// <summary>
    /// This singleton class hold all the reference to non-mono service derived from <see cref="Base.Core.IService"/> and <see cref="Base.Core.ISignal"/>
    /// </summary>
    public class ServiceLocator : SingletonMono<ServiceLocator>
    {
        private IDictionary<Type, IService> m_services = new Dictionary<Type, IService>();

        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var service in m_services)
            {
                if (service.Value.IsInitialize)
                {
                    service.Value.Dispose();
                }
            }
            
            m_services.Clear();
        }

        public static T Get<T>() where T : class, IService
        {
            return Instance.Resolve<T>();
        }

        private T Set<T>() where T : class, IService
        {
            object result = null;
            
            if (!m_services.TryGetValue(typeof(T), out IService item))
            {
                item = Activator.CreateInstance<T>() as IService;

                m_services[typeof(T)] = item;
                result = item;
            }

            if (result == null)
            {
                throw new Exception($"ServiceLocator has no services name {typeof(T)}");
            }

            return result as T;
        }

        public static void Set<T>(T inst) where T : class, IService
        {
            if (!Instance.m_services.ContainsKey(inst.GetType()))
            {
                Instance.m_services[inst.GetType()] = inst as IService;
            }
        }

        public static bool IsSet<T>() where T : class, IService
        {
            return Instance.m_services.ContainsKey(typeof(T));
        }

        private T Resolve<T>() where T : class, IService
        {
            return m_services.TryGetValue(typeof(T), out IService service) ? service as T : Set<T>();
        }

    }
}

