using System.Reflection;
using TMPro;
using UnityEngine;

namespace Base.Cheat
{
    public class InputParameterDisplay : ParameterItemDisplayBase
    {
        [SerializeField] private TMP_InputField m_inputField;
        
        public override void Initialize(ParameterInfo parameterData)
        {
            Active               = true;
            m_value              = parameterData.HasDefaultValue ? parameterData.DefaultValue : null;
            m_parameterName.text = $"{parameterData.Name}:{CheatUtils.GetValueTypeName(parameterData.ParameterType)}";
            m_inputField.text    = m_value?.ToString() ?? string.Empty;
            
            m_inputField.onEndEdit.RemoveAllListeners();
            m_inputField.onEndEdit.AddListener(OnEndEdit);
        }
        public override string GetValue()
        {
            return m_value.ToString();
        }

        private void OnEndEdit(string crrValue)
        {
            m_value = crrValue;
        }
    }
}

