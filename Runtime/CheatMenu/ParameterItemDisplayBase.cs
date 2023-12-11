using System.Reflection;
using Base.Helper;
using TMPro;
using UnityEngine;

namespace Base.Cheat
{
    public abstract class ParameterItemDisplayBase : BaseUI
    {
        [SerializeField] protected TMP_Text m_parameterName;

        protected object m_value;

        public abstract void Initialize(ParameterInfo parameterData);
        
        public abstract string GetValue();
    }
}

