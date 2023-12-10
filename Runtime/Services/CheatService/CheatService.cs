using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Base.Logging;
using Base.Pattern;
using UnityEngine;

namespace Base.Cheat
{
    public class CheatService : Service
    {
        /// <summary>
        /// Used to stores and registers cheat command
        /// </summary>
        private readonly SortedDictionary<string, ICheatCommand> m_commands;

        private IDictionary<Type, WeakReference> m_instanceRegistry = new Dictionary<Type, WeakReference>();

        public CheatService()
        {
            m_commands = new SortedDictionary<string, ICheatCommand>();
        }

        public void Init(string assemblyName)
        {
            Assembly[] assemblies   = System.AppDomain.CurrentDomain.GetAssemblies();
            Assembly   mainAssembly = assemblies.FirstOrDefault(a => a.GetName().Name.Equals(assemblyName));
            if (mainAssembly != null)
            {
                CheatUtils.Scan(mainAssembly.GetTypes());
                IsInitialize = true;
            }
        }

        public void RegisterCommand(ICheatCommand command)
        {
            string key = command.Name.ToLower();

            if (m_commands.ContainsKey(key))
            {
                PDebug.ErrorFormat("[CheatService] A command with the name {0} already exist", key);
                return;
            }

            m_commands[key] = command;
        }

        public void RegisterInstance<T>(T type)
        {
            if (!IsRegisterInstance(typeof(T)))
            {
                m_instanceRegistry[typeof(T)] = new WeakReference(type);
            }
        }

        public void UnRegisterInstance<T>()
        {
            if (m_instanceRegistry.ContainsKey(typeof(T)))
            {
                m_instanceRegistry.Remove(typeof(T));
            }
        }

        public object GetInstance(Type type)
        {
            if (!IsRegisterInstance(type))
            {
                throw new InvalidOperationException(string.Format("[Cheat] No instance of type {0} registered", type));
            }

            return m_instanceRegistry[type].Target;
        }

        public List<ICheatCommand> GetCheatCommands()
        {
            return m_commands.Values.ToList();
        }

        private bool IsRegisterInstance(Type type)
        {
            RemoveDeadReference(type);
            return m_instanceRegistry.ContainsKey(type);
        }
        
        /// <summary>
        /// Remove the reference if it has been destroyed or uninitialized
        /// </summary>
        /// <param name="type"></param>
        private void RemoveDeadReference(Type type)
        {
            if (m_instanceRegistry.ContainsKey(type) && !IsAliveReference(m_instanceRegistry[type]))
            {
                PDebug.InfoFormat("[Cheat] Remove dead reference of type {0}", type.Name);
                m_instanceRegistry.Remove(type);
            }
        }
        
        /// <summary>
        /// Check if whether the instance is uninitialized or destroyed by unity
        /// </summary>
        /// <param name="reference">The instance to check</param>
        /// <returns></returns>
        private bool IsAliveReference(WeakReference reference)
        {
            return reference.IsAlive && (!(reference.Target is UnityEngine.Object target) || target != null);
        }
    }
}

