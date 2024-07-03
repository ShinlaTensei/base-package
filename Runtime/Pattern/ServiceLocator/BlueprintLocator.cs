using System;
using System.Collections.Generic;
using Base.Core;
using Base.Module;

namespace Base.Pattern
{
    public class BlueprintLocator : SingletonMono<BlueprintLocator>
    {
        private IDictionary<Type, IBlueprint> m_blueprints = new Dictionary<Type, IBlueprint>();

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            m_blueprints.Clear();
        }

        public static T Get<T>() where T : class, IBlueprint
        {
            return Instance.Resolve<T>();
        }

        private T Set<T>() where T : class, IBlueprint
        {
            object result = null;
            
            if (!m_blueprints.TryGetValue(typeof(T), out IBlueprint item))
            {
                item = Activator.CreateInstance<T>() as IBlueprint;

                m_blueprints[typeof(T)] = item;
                result = item;
            }

            if (result == null)
            {
                throw new Exception($"BlueprintLocator has no blueprint name {typeof(T)}");
            }

            return result as T;
        }

        public static void Set<T>(T inst) where T : class, IBlueprint
        {
            if (!Instance.m_blueprints.ContainsKey(inst.GetType()))
            {
                Instance.m_blueprints[inst.GetType()] = inst as IBlueprint;
            }
        }
        
        public static bool IsSet<T>() where T : class, IBlueprint
        {
            return Instance.m_blueprints.ContainsKey(typeof(T));
        }

        private T Resolve<T>() where T : class, IBlueprint
        {
            return m_blueprints.TryGetValue(typeof(T), out IBlueprint service) ? service as T : Set<T>();
        }
    }
}