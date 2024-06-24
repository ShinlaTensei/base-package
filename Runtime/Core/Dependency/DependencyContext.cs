using System;
using System.Collections.Generic;
using Base.Helper;
using Base.Logging;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Base.Core
{
    public class DependencyContext
    {
        /// <summary>
        /// Backing field of <see cref="UnityDependencyRegistry"/>
        /// </summary>
        private readonly IDictionary<Type, UnityDependencyObject> m_unityDependencyRegistry = new Dictionary<Type, UnityDependencyObject>();

        private readonly IList<Type> m_pendingToRemoved = new List<Type>();

        /// <summary>
        /// Hold the reference to the register <see cref="UnityEngine.Object"/>
        /// </summary>
        private IDictionary<Type, UnityDependencyObject> UnityDependencyRegistry => m_unityDependencyRegistry;
        
        private IList<Type> PendingToRemoved => m_pendingToRemoved;

        public T Get<T>() where T : MonoBehaviour
        {
            if (!UnityDependencyRegistry.TryGetValue(typeof(T), out UnityDependencyObject dependency))
            {
                try
                {
                    T target = new GameObject(nameof(T)).AddComponent<T>();
                    Register(target);
                }
                catch (Exception e)
                {
                    throw new Exception($"Get Dependency of type {nameof(T)} not found", e);
                }
            }
            
            if (UnityDependencyRegistry.TryGetValue(typeof(T), out dependency))
            {
                return (T)dependency.Target;
            }
            
            throw new KeyNotFoundException("Tried to get dependency type [" + typeof(T) +
                                           "] but no object of that type has been registered.");
        }

        public bool TryGet<T>(out T result)
        {
            if (UnityDependencyRegistry.TryGetValue(typeof(T), out UnityDependencyObject dependency)
                && dependency.Target is T entry)
            {
                result = entry;
                // this expression is _not_ always true, because Unity objects cant
                // null check using is operators.
                return entry != null;
            }
            
            PDebug.Warn("Tried to get dependency type [" + typeof(T) + "] but no object of that type has been registered.");
            result = default;
            return false;
        }

        public void Register<T>(T obj) where T : Object
        {
            Register(obj, typeof(T), obj);
        }

        public void Register(Object obj, Type contract)
        {
            Register(obj, contract, obj);
        }

        public void Register(object key, Type contract, UnityEngine.Object sponsorObject)
        {
            //Make sure the object registered is an instance of type contract
            if (!contract.IsInstanceOfType(key))
            {
                throw new InvalidCastException("Attempt to register object:" + key
                                                                             + " under type: " + contract.ToString() + ". These types are not convertible");
            }
            
            if (UnityDependencyRegistry.TryGetValue(contract, out UnityDependencyObject entry))
            {
                PDebug.Warn("[DependencyContext] You are trying to register an exist dependency. This is not allowed!");
            }

            entry = new UnityDependencyObject(contract, key, sponsorObject);
            UnityDependencyRegistry[contract] = entry;
        }

        public void UnRegister(object toUnload)
        {
            foreach (KeyValuePair<Type, UnityDependencyObject> kvp in UnityDependencyRegistry)
            {
                object obj = kvp.Value.Target;
                if (obj == toUnload)
                {
                    PendingToRemoved.AddIfNotContains(kvp.Key);
                }
            }

            foreach (Type toRemoved in PendingToRemoved)
            {
                if (UnityDependencyRegistry.ContainsKey(toRemoved))
                {
                    UnityDependencyRegistry.Remove(toRemoved);
                }
            }
            
            PendingToRemoved.Clear();
        }
    }
}