using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Base.Helper
{
    using System;
    using UnityEngine;

    [AttributeUsage(AttributeTargets.Field)]
    public class BitFlagAttribute : PropertyAttribute
    {
        Type     m_type;
        string[] m_enumNames;
        int      m_length;
        bool     m_hasNull     = false;
        int      m_valuePerRow = 3;

        public Type     Type        { get { return m_type; } }
        public string[] EnumNames   { get { return m_enumNames; } }
        public int      Length      { get { return m_length; } }
        public int      ValuePerRow { get { return m_valuePerRow; } }
        public bool     HasNull     { get { return m_hasNull; } }

        public BitFlagAttribute(Type type, int valuePerRow = 3, bool hasNull = false)
        {
            m_type        = type;
            m_enumNames   = Enum.GetNames(type);
            m_length      = m_enumNames.Length;
            m_hasNull     = hasNull;
            m_valuePerRow = valuePerRow;
        }
    }
}