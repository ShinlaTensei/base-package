using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Base.Helper
{
    public static class ArrayExtensions
    {
        public static void ForEach(this Array array, Action<Array, int[]> action)
        {
            if (array.LongLength != 0L)
            {
                ArrayTraverse arrayTraverse = new ArrayTraverse(array);
                do
                {
                    action(array, arrayTraverse.Position);
                }
                while (arrayTraverse.Step());
            }
        }

        public static void Clear(this Array array, int startIndex, int count)
        {
            Array.Clear(array, startIndex, count);
        }

        public static void ClearAll(this Array array) => Clear(array, 0, array.Length);
    }
}

