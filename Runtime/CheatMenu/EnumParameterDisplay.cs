using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace Base.Cheat
{
    public class EnumParameterDisplay : ParameterItemDisplayBase
    {
        [SerializeField] private TMP_Dropdown m_dropdown;
        public override void Initialize(ParameterInfo parameterData)
        {
            Active               = true;
            m_value              = parameterData.HasDefaultValue ? parameterData.DefaultValue : null;
            m_parameterName.text = $"{parameterData.Name}:{parameterData.ParameterType.Name}";
        }

        public void Initialize(ParameterInfo parameterInfo, List<string> options)
        {
            Initialize(parameterInfo);
            
            m_dropdown.ClearOptions();
            m_dropdown.AddOptions(options);
            
            m_dropdown.onValueChanged.RemoveAllListeners();
            m_dropdown.onValueChanged.AddListener(OnValueChanged);
        }

        public void Initialize(ParameterInfo parameterInfo, Type enumType)
        {
            string[] valueList = Enum.GetNames(enumType);
            Initialize(parameterInfo, valueList.ToList());
        }
        
        public override string GetValue()
        {
            m_value ??= m_dropdown.options[m_dropdown.value].text;
            return m_value.ToString();
        }

        private void OnValueChanged(int index)
        {
            m_value = m_dropdown.options[index].text;
        }
    }
}

