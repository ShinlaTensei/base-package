using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Base
{
    public class InputParamUI : MonoBehaviour
    {
        [SerializeField] TMP_Text   m_paramName;
        [SerializeField] InputField m_mainInput;
        [SerializeField] Toggle     m_toggle;

        string m_inputType;

        public InputField MainInput => m_mainInput;
        public Toggle     Toggle1   => m_toggle;

        public void InitUI(string name, string inputType, object value)
        {
            m_paramName.text = name;
            m_inputType      = inputType;
            if (inputType == nameof(Boolean))
            {
                m_toggle.gameObject.SetActive(true);
                m_mainInput.gameObject.SetActive(false);
                m_toggle.isOn = (bool) value;
            }
            else
            {
                m_toggle.gameObject.SetActive(false);
                m_mainInput.gameObject.SetActive(true);
                m_mainInput.text = value.ToString();
            }
        }

        public void ShowNotify()
        {
            //Toast.ShowToast($"Require a {m_inputType} value!");
        }
    }
}
