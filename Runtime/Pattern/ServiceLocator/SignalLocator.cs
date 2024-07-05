using System;
using System.Collections.Generic;
using Base.Core;

namespace Base.Pattern
{
    public class SignalLocator : SingletonMono<SignalLocator>
    {
        private IDictionary<Type, ISignal> m_signals = new Dictionary<Type, ISignal>();

        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var signal in m_signals)
            {
                signal.Value.RemoveAllListener();
            }
            
            m_signals.Clear();
        }

        public static T Get<T>() where T : class, ISignal
        {
            return Instance.Resolve<T>();
        }

        private T Set<T>() where T : class, ISignal
        {
            object result = null;
            
            if (!m_signals.TryGetValue(typeof(T), out ISignal item))
            {
                item = Activator.CreateInstance<T>() as ISignal;

                m_signals[typeof(T)] = item;
                result = item;
            }

            if (result == null)
            {
                throw new Exception($"SignalLocator has no signals name {typeof(T)}");
            }

            return result as T;
        }

        public static void Set<T>(T inst) where T : class, ISignal
        {
            if (!Instance.m_signals.ContainsKey(inst.GetType()))
            {
                Instance.m_signals[inst.GetType()] = inst as ISignal;
            }
        }
        
        public static bool IsSet<T>() where T : class, ISignal
        {
            return Instance.m_signals.ContainsKey(typeof(T));
        }

        public static bool HasListener<T>() where T : class, ISignal
        {
            if (Instance.m_signals.TryGetValue(typeof(T), out ISignal signal))
            {
                return signal.HasListener();
            }

            return false;
        }

        private T Resolve<T>() where T : class, ISignal
        {
            return m_signals.TryGetValue(typeof(T), out ISignal service) ? service as T : Set<T>();
        }
    }
}