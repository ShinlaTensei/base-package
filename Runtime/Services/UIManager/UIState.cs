using System;
using System.Collections;
using System.Collections.Generic;
using Base.Helper;
using Base.Module;
using UnityEngine;

namespace Base
{
    [Serializable]
    public record UIState
    {
        [SerializeField]                    private string           m_stateName;
        [SerializeField, CustomClassDrawer] private List<UISubState> m_subStates = new List<UISubState>();
    }
    
    [Serializable]
    public record UISubState
    {
        [SerializeField] private string m_subStateName;
    }
    
    [Serializable]
    public record UIViewReference
    {
        private const string AssemblyName = "Assembly-Csharp";
        
        [DerivedFlag(typeof(UIView), AssemblyName)]
        [SerializeField] private string m_viewType;
    }
}

