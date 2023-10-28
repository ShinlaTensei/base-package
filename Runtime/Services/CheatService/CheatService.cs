using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Base.Logging;
using Base.Pattern;
using UnityEngine;

namespace Base.Cheat
{
    public class CheatService : IService
    {
        /// <summary>
        /// Used to stores and registers cheat command
        /// </summary>
        private readonly SortedDictionary<string, ICheatCommand> _commands;

        public CheatService()
        {
            _commands = new SortedDictionary<string, ICheatCommand>();
        }
        
        public void Dispose()
        {
            
        }
        public void Init()
        {
            CheatUtils.Scan(Assembly.GetAssembly(typeof(CheatCommandAttribute)).GetTypes());
        }

        public void RegisterCommand(ICheatCommand command)
        {
            string key = command.Name.ToLower();

            if (_commands.ContainsKey(key))
            {
                PDebug.ErrorFormat("[CheatService] A command with the name {0} already exist", key);
                return;
            }

            _commands[key] = command;
        }
    }
}

