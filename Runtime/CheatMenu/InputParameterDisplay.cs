using System.Reflection;
using TMPro;
using UnityEngine;

namespace Base.Cheat
{
    public class InputParameterDisplay : ParameterItemDisplayBase
    {
        [SerializeField] protected TMP_Text m_parameterNameText;

        private object m_value;
        public override void Initialize(ParameterInfo parameterData)
        {
            m_value = parameterData.HasDefaultValue ? parameterData.DefaultValue : null;
        }
        public override string GetValue()
        {
            return m_value.ToString();
        }
    }
}

