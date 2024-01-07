#region Header
// Date: 15/11/2023
// Created by: Huynh Phong Tran
// File name: ColorSwitcher.cs
#endregion

using System;
using System.Collections.Generic;
using Base.Logging;
using Base.Utilities;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Base.Helper
{
    public class UIColorSwitcher : MonoBehaviour, ISwitcher<Graphic>
    {
        [SerializeField] private List<Color> m_colors = new List<Color>();

        private int  m_selectedIndex = -1;
        private bool m_isInitialized;

        public Graphic Binding { get; private set; }

        [SerializeField, OnValueChanged(nameof(OnSimulateChanged))] private bool m_simulate = false;
        [SerializeField, Condition(nameof(m_simulate), true)]
        private int m_simulateIndex = 0;

        private void Awake()
        {
            Initialized();
        }

        private void Initialized()
        {
            Binding        = GetComponent<Graphic>();
            m_isInitialized = true;
        }
        public void Switch(int index)
        {
            if (!m_isInitialized) Initialized();
            if (m_selectedIndex == index) return;

            Color selectedColor = Binding.color;
            try
            {
                selectedColor = m_colors[index];
            }
            catch (IndexOutOfRangeException e)
            {
                PDebug.Error(e, e.Message);
            }
            finally
            {
                m_selectedIndex = index;
                Binding.color  = selectedColor;
            }

        }
        
        [Button(ButtonSizes.Small, ButtonStyle.Box, Name = "Simulate"), UsedImplicitly]
        private void UpdateColor()
        {
            if (m_simulate)
            {
                Switch(m_simulateIndex);
            }
        }
        [UsedImplicitly]
        private void OnSimulateChanged()
        {
            if (!m_simulate)
            {
                Switch(0);
            }
        }
    }
}