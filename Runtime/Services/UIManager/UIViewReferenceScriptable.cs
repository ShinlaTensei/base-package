using System;
using System.Collections;
using System.Collections.Generic;
using Base.Helper;
using UnityEngine;

namespace Base
{
    [CreateAssetMenu(menuName = "Base Framework/UI Manager/UI View Reference")]
    public class UIViewReferenceScriptable : ScriptableObject
    {
        [SerializeField] private List<UIViewReference> m_references = new List<UIViewReference>();
    }
}

