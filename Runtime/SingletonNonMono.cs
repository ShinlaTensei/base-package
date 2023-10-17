using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Base
{
    public class SingletonNonMono<T> where T : class
    {
        private static Lazy<T> m_instance = new Lazy<T>(Activator.CreateInstance<T>);
        
        protected static bool m_ShuttingDown = false;

        private static object m_Lock = new object();

        public static T Instance
        {
            get
            {
                if (m_ShuttingDown)
                {
                    Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                                     "' already destroyed. Returning null.");
                    return null;
                }

                lock (m_Lock)
                {
                    return m_instance.Value;
                }
                
            }
        }
    }
}

