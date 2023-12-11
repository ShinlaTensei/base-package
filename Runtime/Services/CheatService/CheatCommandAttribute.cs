#region Header
// Date: 29/10/2023
// Created by: Huynh Phong Tran
// File name: CheatCommandAttribute.cs
#endregion

using System;
using JetBrains.Annotations;

namespace Base.Cheat
{
    /// <summary>
    /// Provides an attribute to register a standard console command with Uniterm.
    /// 
    /// Anywhere in code you want a method to be callable for a cheat you add the cheatCommandAttribute over the method with all required parameters.
    /// The CheatCommandAttribute is also processed inside the script processor and Ai.
    /// 
    /// Commands are generally methods in any class of the project which change the game
    /// state or its configuration for the purposes of debugging, altering uncommon configuration
    /// properties or cheating (if left activated in production).
    /// </summary>
    [AttributeUsage(AttributeTargets.Method), MeansImplicitUse]
    public class CheatCommandAttribute : Attribute
    {
        /// <summary>
        /// Name of the command
        ///
        /// This name shouldn't contain spaces as it is directly used when parsing messages entered
        /// in the console
        /// </summary>
        public string Name          { get; private set; }
        /// <summary>
        /// A short description of the command usage
        ///
        /// Should write it with short and understandable information
        /// </summary>
        public string Description   { get; private set; }
        /// <summary>
        /// The cheat category of the command to show in the UI
        /// </summary>
        public string CheatCategory { get; private set; }

        public CheatCommandAttribute(string name, string description = "", string category = "")
        {
            Name          = name;
            Description   = description;
            CheatCategory = category;
        }
    }
}