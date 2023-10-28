#region Header
// Date: 29/10/2023
// Created by: Huynh Phong Tran
// File name: ICheatCommand.cs
#endregion

using System;
using System.Linq;
using System.Reflection;
using Base.Logging;

namespace Base.Cheat
{
    /// <summary>
    /// Cheat command registration
    /// </summary>
    public interface ICheatCommand
    {
        /// <summary>
        /// Command name
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Determine if the command is static command
        /// </summary>
        bool IsStatic { get; }
        
        /// <summary>
        /// Execute the command
        /// </summary>
        /// <param name="instance">the instance to run the command on</param>
        /// <param name="callParameters">the parameters to run the command with</param>
        /// <returns></returns>
        object Execute(object instance, string[] callParameters);
        
        /// <summary>
        /// The type that this command belong to
        /// </summary>
        Type DeclaringType { get; }
        
        /// <summary>
        /// Returns the parameter information for this command
        /// </summary>
        /// <returns></returns>
        ParameterInfo[] GetParameters();
    }

    public class MethodCheatCommand : ICheatCommand
    {
        public  string                Name          { get; }
        public  bool                  IsStatic      { get; }
        public  Type                  DeclaringType { get; }
        private CheatCommandAttribute Attribute     { get; }
        private MethodInfo            MethodInfo    { get; }

        public MethodCheatCommand(MethodInfo methodInfo, CheatCommandAttribute attribute)
        {
            Attribute     = attribute;
            Name          = attribute.Name;
            MethodInfo    = methodInfo;
            DeclaringType = methodInfo.DeclaringType;
            IsStatic      = methodInfo.IsStatic;
        }
        
        public object Execute(object instance, string[] callParameters)
        {
            ParameterInfo[] parameterInfos = GetParameters();
            
            int mandatoryParam = parameterInfos.Count(info => !info.IsOptional);

            if (callParameters.Length < mandatoryParam || callParameters.Length > parameterInfos.Length)
            {
                PDebug.ErrorFormat("[CheatService] Execute method {0} error: Number of argument does not match!", Name);
                return null;
            }
            
            object[] parameters = new object[parameterInfos.Length];

            for (int i = 0; i < parameterInfos.Length; i++)
            {
                ParameterInfo info = parameterInfos[i];
                if (i >= callParameters.Length)
                {
                    parameters[i] = info.DefaultValue;
                    continue;
                }

                parameters[i] = CheatUtils.ConvertTo(info.ParameterType, callParameters[i]);
            }

            try
            {
                return MethodInfo.Invoke(instance, parameters);
            }
            catch (Exception e)
            {
                PDebug.Error(e, e.Message);
                return null;
            }
        }
        public ParameterInfo[] GetParameters()
        {
            return MethodInfo.GetParameters();
        }
    }
}