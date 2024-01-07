#region Header
// Date: 04/01/2024
// Created by: Huynh Phong Tran
// File name: ListItemSelectorAttribute.cs
#endregion

using System;

namespace Base.CustomAttribute
{
    public class ListItemSelectorAttribute : Attribute
    {
        public string SetSelectedMethod;

        public ListItemSelectorAttribute(string setSelectedMethod)
        {
            this.SetSelectedMethod = setSelectedMethod;
        }
    }
}