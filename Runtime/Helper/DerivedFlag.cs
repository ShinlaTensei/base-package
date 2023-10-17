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
        private Type m_baseType;

        public Type BaseType => m_baseType;
        public DerivedFlagAttribute(Type baseType)
        {
            m_baseType = baseType;
        }
    }
}