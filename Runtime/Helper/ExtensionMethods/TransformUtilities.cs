using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Base.Helper
{
    public static class TransformUtilities
    {
        #region Scale

        public static void SetScale(this Transform target, float endValue)
        {
            target.localScale = new Vector3(endValue, endValue, endValue);
        }

        public static void SetScale(this Transform target, float x, float y, float z)
        {
            target.localScale = new Vector3(x, y, z);
        }

        #endregion
        
        #region Position

        public static void SetPosX(this Transform target, float xPos)
        {
            Vector3 position = target.position;
            target.position = new Vector3(xPos, position.y, position.z);
        }

        public static void SetPosY(this Transform target, float yPos)
        {
            Vector3 position = target.position;
            target.position = new Vector3(position.x, yPos, position.z);
        }

        public static void SetPosZ(this Transform target, float zPos)
        {
            Vector3 position = target.position;
            target.position = new Vector3(position.x, position.y, zPos);
        }

        public static void SetPosition(this Transform target, Vector3 position)
        {
            target.position = position;
        }

        public static void SetPosition(this Transform target, float x, float y, float z)
        { 
            target.SetPosition(new Vector3(x, y, z));
        }

        public static void SetLocalPosition(this Transform target, Vector3 newLocalPos)
        {
            target.localPosition = newLocalPos;
        }
        
        public static void SetLocalPosition(this Transform target, float x, float y, float z)
        {
            target.SetLocalPosition(new Vector3(x, y, z));
        }

        #endregion
        
        #region Child Interactions

        public static List<Transform> GetAllChildren(this Transform target)
        {
            List<Transform> children = new List<Transform>();
            foreach (Transform child in target)
            {
                children.Add(child);
            }

            return children;
        }

        public static void DestroyAllChildren(this Transform target, bool isImmediate = false)
        {
            foreach (Transform child in target)
            {
                if (!isImmediate) Object.Destroy(child.gameObject);
                else Object.DestroyImmediate(child.gameObject);
            }
        }
        
        /// <summary>
        /// Get the index of child in parent transform
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        public static int IndexOf(this Transform parent, Transform child)
        {
            if (child.IsChildOf(parent))
            {
                int childCount = parent.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    if (child.Equals(parent.GetChild(i)))
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        public static void RemoveChildren(this Transform target, Transform child, Transform newParent = null)
        {
            if (newParent)
            {
                child.SetParent(newParent);
            }
            else
            {
                child.SetParent(target.root);
            }
        }

        public static void RemoveAllChildren(this Transform target, Transform newParent = null)
        {
            foreach (Transform child in target)
            {
                target.RemoveChildren(child, newParent);
            }
        }

        public static void RemoveFromParent(this Transform target)
        {
            target.SetParent(target.root);
        }
        
        public static T FindChildRecursive<T>(this Transform transform, string name, bool recursive = true)
        {
            foreach (Transform tm in transform)
            {
                if (tm.name == name && tm.GetComponent<T>() != null) { return tm.GetComponent<T>(); }
                if (recursive)
                {
                    T res = FindChildRecursive<T>(tm, name);
                    if (res != null) { return res; }
                }
            }
            return default(T);
        }
        public static T FindChildRecursive<T>(this Component obj, string name, bool recursive = true)
        {
            foreach (Transform o in obj.transform)
            {
                if (o.name == name && o.gameObject.GetComponent<T>() != null) { return o.gameObject.GetComponent<T>(); }
                if (recursive)
                {
                    T res = FindChildRecursive<T>(o, name);
                    if (res != null) { return res; }
                }
            }
            return default(T);
        }
        public static T FindChildRecursive<T>(this GameObject obj, string name, bool recursive = true)
        {
            foreach (Transform o in obj.transform)
            {
                if (o.name == name && o.gameObject.GetComponent<T>() != null) { return o.gameObject.GetComponent<T>(); }
                if (recursive)
                {
                    T res = FindChildRecursive<T>(o.gameObject, name);
                    if (res != null) { return res; }
                }
            }
            return default(T);
        }
        public static GameObject FindChildRecursive(this GameObject obj, string name, bool recursive = true)
        {
            foreach (Transform o in obj.transform)
            {
                if (o.gameObject.name == name) { return o.gameObject; }
                if (recursive)
                {
                    GameObject res = FindChildRecursive(o.gameObject, name);
                    if (res != null) { return res; }
                }
            }
            return null;
        }

        #endregion

    }
}
