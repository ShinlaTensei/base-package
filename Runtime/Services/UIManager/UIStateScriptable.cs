#region Header
// Date: 18/10/2023
// Created by: Huynh Phong Tran
// File name: UIStateScriptable.cs
#endregion

using System;
using System.Collections.Generic;
using Base.Helper;
using Base.Module;
using NaughtyAttributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Base
{
    [CreateAssetMenu(menuName = "Base Framework/UI Manager/UI State Manager")]
    public class UIStateScriptable : ScriptableObject
    {
        [SerializeField]                    private UIViewReferenceScriptable m_viewReferenceScriptable;
        [SerializeField, CustomClassDrawer] private List<UIState>             m_states = new List<UIState>();
    }
}