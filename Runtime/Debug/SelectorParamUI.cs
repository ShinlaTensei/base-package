using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Base
{
    public class SelectorParamUI : MonoBehaviour
    {
        [SerializeField] TMP_Text     m_paramName;
        [SerializeField] TMP_Dropdown m_mainInput;

        string m_inputType;

        public TMP_Text     ParamName => m_paramName;
        public TMP_Dropdown MainInput => m_mainInput;

        public void InitUI(string name, string inputType, object value)
        {
            m_paramName.text = name;
            m_inputType      = inputType;
        }
    }
}

