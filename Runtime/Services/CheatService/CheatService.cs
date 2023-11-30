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

        private const string AssemblyName = "Assembly-CSharp";

        public CheatService()
        {
            m_commands = new SortedDictionary<string, ICheatCommand>();
        }
        
        public override void Dispose()
        {
            
        }
        public override void Init()
        {
            Assembly[] assemblies   = System.AppDomain.CurrentDomain.GetAssemblies();
            Assembly   mainAssembly = assemblies.FirstOrDefault(a => a.GetName().Name.Equals(AssemblyName));
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

        public List<ICheatCommand> GetCheatCommands()
        {
            return m_commands.Values.ToList();
        }
    }
}

