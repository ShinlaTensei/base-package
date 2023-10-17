using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Base.Helper
{
    public static class Vector3ExtensionMethods
    {
        public static bool IsNaN(this Vector3 target)
        {
            return (float.IsNaN(target.x) || float.IsNaN(target.y) || float.IsNaN(target.z));
        }

        public static Vector3 Add(this Vector3 a, float value, Vector3 axis)
        {
            Vector3 newAxis = new Vector3();
            
            newAxis.x = Mathf.Clamp(axis.x, -1, 1);
            newAxis.y = Mathf.Clamp(axis.y, -1, 1);
            newAxis.z = Mathf.Clamp(axis.z, -1, 1);

            newAxis.x = Mathf.RoundToInt(newAxis.x);
            newAxis.y = Mathf.RoundToInt(newAxis.y);
            newAxis.z = Mathf.RoundToInt(newAxis.z);
            
            return new Vector3(a.x + (newAxis.x * value), a.y + newAxis.y * value, a.z + newAxis.z * value);
        }
    }
}

