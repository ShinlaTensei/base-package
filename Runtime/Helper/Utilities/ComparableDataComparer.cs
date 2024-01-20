#region Header
// Date: 20/01/2024
// Created by: Huynh Phong Tran
// File name: ComparableDataComparer.cs
#endregion

using System;
using System.Collections.Generic;

namespace Base.Helper
{
    public class ComparableDataComparer : IEqualityComparer<IComparable>
    {
        public bool Equals(IComparable x, IComparable y)
        {
            if (x is null || y is null)
            {
                return false;
            }
            if (x.CompareTo(y) != 0)
            {
                return false;
            }

            return true;
        }
        public int GetHashCode(IComparable obj)
        {
            return obj.GetHashCode();
        }
    }
}