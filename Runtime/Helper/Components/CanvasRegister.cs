#region Header
// Date: 20/03/2024
// Created by: Huynh Phong Tran
// File name: CanvasRegister.cs
#endregion

using System;
using Base.Cheat;
using Base.Pattern;

namespace Base.Helper
{
    public class CanvasRegister : BaseMono
    {
        protected override void Start()
        {
            UIViewManager viewManager = ServiceLocator.Get<UIViewManager>();
            if (viewManager != null && viewManager.IsInitialize)
            {
                viewManager.RegisterCanvas(this);
            }
        }
    }
}