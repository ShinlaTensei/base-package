using System;
using Base.Helper;
using UnityEngine;

namespace Base
{
    /// <summary>
    /// Inherit from this base class to create a singleton.
    /// e.g. public class MyClassName : Singleton<MyClassName> {}
    /// </summary>
    public class SingletonMono<T> : BaseMono where T : BaseMono
    {
        [SerializeField] private bool isPersistent;
        // Check to see if we're about to be destroyed.
        protected static bool m_ShuttingDown = false;

        private static object m_Lock = new object();
        protected static T m_Instance;

        public static bool ShuttingDown => m_ShuttingDown;

        /// <summary>
        /// Access singleton instance through this propriety.
        /// </summary>
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
                    if (m_Instance == null)
                    {
                        if (m_Instance == null)
                        {
                            // Need to create a new GameObject to attach the singleton to.
                            var singletonObject = new GameObject();
                            m_Instance = singletonObject.AddComponent<T>();
                            singletonObject.name = typeof(T).ToString() + " (Singleton)";

                            // Make instance persistent.
                            DontDestroyOnLoad(singletonObject);
                        }
                    }

                    return m_Instance;
                }
            }
        }

        protected virtual void Awake()
        {
            m_Instance = this as T;

            if (isPersistent)
            {
                DontDestroyOnLoad(this);
            }
        }

        protected virtual void OnDestroy()
        {
            m_ShuttingDown = true;
            m_Instance = null;
        }
    }
}