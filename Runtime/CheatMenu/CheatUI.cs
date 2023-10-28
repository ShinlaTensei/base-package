#region Header
// Date: 15/11/2023
// Created by: Huynh Phong Tran
// File name: CheatUI.cs
#endregion

using Base.Cheat;
using Base.Helper;
using Base.Pattern;
using TMPro;
using UnityEngine;

namespace Base
{
    public class CheatUI : UIView
    {
        [SerializeField] private TMP_Dropdown m_dropdown;

        private CheatService m_cheatService;
        protected override void Awake()
        {
            m_cheatService = ServiceLocator.Get<CheatService>();
        }
        public override void Populate<T>(T viewData)
        {
            
        }
    }
}