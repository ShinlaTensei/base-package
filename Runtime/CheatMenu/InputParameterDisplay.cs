using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Base
{
    public class InputParameterDisplay : ParameterItemDisplayBase
    {
        [SerializeField] protected TMP_Text m_parameterNameText;
        public override void Initialize(CheatParameterData parameterData)
        {
            
        }
        public override string GetValue()
        {
            return string.Empty;
        }
    }
}

