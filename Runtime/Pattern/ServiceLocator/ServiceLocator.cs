using System;
using System.Collections.Generic;
using Base.Core;
using Base.Helper;
using Base.Logging;
using UnityEngine;

namespace Base.Pattern
{
    /// <summary>
    /// This singleton class hold all the reference to non-mono service derived from <see cref="Base.Core.IService"/> and <see cref="Base.Core.ISignal"/>
    /// </summary>
    public class ServiceLocator : SingletonMono<ServiceLocator>
    {
        private IDictionary<Type, IService> m_services = new Dictionary<Type, IService>();
        private IDictionary<Type, ISignal> m_signals = new Dictionary<Type, ISignal>();

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
            m_signals.Clear();
        }

        public static T Get<T>() where T : class
        {
            return Instance.Resolve<T>();
        }

        private T Set<T>() where T : class
        {
            object result = null;
            if (!m_services.TryGetValue(typeof(T), out IService item))
            {
                item = Activator.CreateInstance<T>() as IService;

                m_services[typeof(T)] = item;
                result = item;
            }
            
            if (!m_signals.TryGetValue(typeof(T), out ISignal signal))
            {
                signal = Activator.CreateInstance<T>() as ISignal;
                result = signal;
            }

            if (result == null)
            {
                throw new Exception($"ServiceLocator has no services or signals name {typeof(T)}");
            }

            return result as T;
        }

        public static void Set<T>(T inst) where T : class
        {
            if (!Instance.m_services.ContainsKey(inst.GetType()))
            {
                Instance.m_services[inst.GetType()] = inst as IService;
            }
            else if (!Instance.m_signals.ContainsKey(inst.GetType()))
            {
                Instance.m_signals[inst.GetType()] = inst as ISignal;
            }
        }

        private T Resolve<T>() where T : class
        {
            object result = m_services.TryGetValue(typeof(T), out IService item) ? item :
                m_signals.TryGetValue(typeof(T), out ISignal signal) ? signal : Set<T>();

            return result as T;
        }

    }
}

