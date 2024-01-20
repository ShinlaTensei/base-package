#region Header
// Date: 16/01/2024
// Created by: Huynh Phong Tran
// File name: PageSliderAttribute.cs
#endregion

using System;

namespace Base.CustomAttribute
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class PageSliderAttribute : System.Attribute
    {
    }
}