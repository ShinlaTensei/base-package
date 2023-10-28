#region Header
// Date: 29/10/2023
// Created by: Huynh Phong Tran
// File name: CheatScanner.cs
#endregion

using System;
using System.Globalization;
using System.Reflection;
using Base.Pattern;

namespace Base.Cheat
{
    public static class CheatUtils
    {
        public static void Scan(Type[] types)
        {
            CheatService cheatService = ServiceLocator.Get<CheatService>();
            
            const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

            foreach (Type type in types)
            {
                foreach (MethodInfo methodInfo in type.GetMethods(bindingFlags))
                {
                    object[] commandAttribute = methodInfo.GetCustomAttributes(typeof(CheatCommandAttribute), false);

                    if (commandAttribute.Length == 0)
                    {
                        continue;
                    }

                    if (commandAttribute.Length > 0)
                    {
                        CheatCommandAttribute cheatCommandAttribute = (CheatCommandAttribute)commandAttribute[0];
                        cheatService.RegisterCommand(new MethodCheatCommand(methodInfo, cheatCommandAttribute));
                    }
                }
            }
        }
        /// <summary>
        /// Converts a string value to the given type, or throws an exception if no conversion exists
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object ConvertTo(Type targetType, string value)
        {
            if (targetType == typeof(string))
            {
                return value;
            }

            if (targetType == typeof(int))
            {
                return Convert.ToInt32(value);
            }

            if (targetType == typeof(float))
            {
                return float.Parse(value, CultureInfo.InvariantCulture);
            }

            if (targetType.IsEnum)
            {
                return Enum.Parse(targetType, value);
            }

            if (targetType == typeof(bool))
            {
                return bool.Parse(value);
            }

            throw new ArgumentException(string.Format("Cannot convert value \"{0}\" to type {1}", value, targetType.Name));
        }
    }
}