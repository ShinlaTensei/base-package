#region Header
// Date: 11/10/2023
// Created by: Huynh Phong Tran
// File name: UIViewManager.UIState.cs
#endregion

using System;
using System.Collections.Generic;

namespace Base
{
    public record UIState
    {
        private string            m_stateName;
        private IList<UISubState> m_subStates = new List<UISubState>();
        
        public string            StateName => m_stateName;
        public IList<UISubState> SubStates => m_subStates;
    }

    public record UISubState
    {
        private string           m_subStateName = String.Empty;
        private IList<UIElement> m_elements     = new List<UIElement>();
        
        public string           SubStateName => m_subStateName;
        public IList<UIElement> Elements     => m_elements;
    }

    public record UIElement
    {
        private string        m_elementName = String.Empty;
        private ElementStatus m_status      = ElementStatus.Visible;

        public string        ElementName => m_elementName;
        public ElementStatus Status      => m_status;
    }

    public enum ElementStatus
    {
        Visible, Hidden
    }
    public partial class UIViewManager
    {
        private IDictionary<string, UIState> m_states = new Dictionary<string, UIState>();
    }
}