#region Header
// Date: 02/08/2023
// Created by: Huynh Phong Tran
// File name: DerivedFlag.cs
#endregion

using System;
using UnityEngine;

namespace Base.Helper
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DerivedFlagAttribute : PropertyAttribute
    {
        private Type   m_baseType     = null;
        private string m_assemblyName = string.Empty;

        public Type   BaseType     => m_baseType;
        public string AssemblyName => m_assemblyName;
        public DerivedFlagAttribute(Type baseType)
        {
            m_baseType = baseType;
        }

        public DerivedFlagAttribute(Type baseType, string assemblyName)
        {
            m_baseType     = baseType;
            m_assemblyName = assemblyName;
        }
    }
}