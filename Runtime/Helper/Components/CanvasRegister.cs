#region Header
// Date: 20/03/2024
// Created by: Huynh Phong Tran
// File name: CanvasRegister.cs
#endregion

using System;
using Base.Cheat;
using Base.Pattern;
using Base.Core;

namespace Base.Helper
{
    public class CanvasRegister : BaseMono
    {
        protected override void Start()
        {
            UIViewManager viewManager = BaseContextRegistry.TryGetOrCreateContext(CoreContext.GLOBAL_CONTEXT).Get<UIViewManager>();
            if (viewManager != null && viewManager.IsInitialize)
            {
                viewManager.RegisterCanvas(this);
            }
        }
    }
}