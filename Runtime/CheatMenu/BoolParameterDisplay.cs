using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Base.Cheat
{
    public class BoolParameterDisplay : ParameterItemDisplayBase
    {
        [SerializeField] private Toggle m_toggle;
        public override void Initialize(ParameterInfo parameterData)
        {
            Active               = true;
            m_value              = parameterData.HasDefaultValue ? parameterData.DefaultValue : false;
            m_parameterName.text = $"{parameterData.Name}:bool";
            m_toggle.isOn        = m_value != null && (bool)m_value;
            m_toggle.onValueChanged.RemoveAllListeners();
            m_toggle.onValueChanged.AddListener(OnValueChanged);
        }
        
        public override string GetValue()
        {
            return m_value.ToString();
        }

        private void OnValueChanged(bool isOn)
        {
            m_value = isOn;
        }
    }
}

