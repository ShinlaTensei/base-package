#region Header
// Date: 14/01/2024
// Created by: Huynh Phong Tran
// File name: MutableValue.cs
#endregion

using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Base.Module
{
    [Serializable]
    public class MutableValue<T> where T : IComparable
    {
        private T m_value;
        
        private T m_oldValue;

        public bool HasValueChanged => OldValue.CompareTo(Value) != 0;
        public T Value
        {
            get => m_value;
            set
            {
                OldValue = m_value;
                m_value  = value;
            }
        }

        public T OldValue
        {
            get => m_oldValue;
            private set => m_oldValue = value;
        }

        public MutableValue()
        {
            
        }

        public MutableValue(T defaultValue)
        {
            Value = defaultValue;
        }
    }
}