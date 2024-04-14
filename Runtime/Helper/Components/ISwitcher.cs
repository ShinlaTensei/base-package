#region Header
// Date: 15/11/2023
// Created by: Huynh Phong Tran
// File name: ISwitcher.cs
#endregion

using System;
using UnityEngine;

namespace Base.Helper
{
    public interface ISwitcher<T> where T : Component
    {
        T Binding { get; }

        void Switch(int index);
    }

    public abstract class Switcher<T> : BaseMono, ISwitcher<T> where T : Component
    {
        /// <summary>
        /// Backing field of <see cref="Binding"/>
        /// </summary>
        private T m_binding;
        /// <summary>
        /// The binding component
        /// </summary>
        public T Binding
        {
            get
            {
                if (m_binding == null)
                {
                    m_binding = this.GetOrAddComponent<T>();
                }

                return m_binding;
            }
        }

        public abstract void Switch(int index);
    }
}