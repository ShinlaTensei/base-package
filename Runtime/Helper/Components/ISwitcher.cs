#region Header
// Date: 15/11/2023
// Created by: Huynh Phong Tran
// File name: ISwitcher.cs
#endregion

using UnityEngine;

namespace Base.Helper
{
    public interface ISwitcher<T> where T : Component
    {
        T Binding { get; }

        void Switch(int index);
    }
}