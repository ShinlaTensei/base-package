using System;
using System.Collections.Generic;
using System.Threading;
using Base.Helper;
using Base.Services;
using UnityEngine;

namespace Base.Pattern
{
    public class ServiceLocator : SingletonNonMono<ServiceLocator>
    {
        private IDictionary<Type, object> m_class;

        public ServiceLocator()
        {
            LazyInitializer.EnsureInitialized(ref m_class, () => new Dictionary<Type, object>());
        }

        public static T Get<T>() where T : class
        {
            return Instance.Resolve<T>();
        }

        private T Set<T>() where T : class
        {
            if (!m_class.TryGetValue(typeof(T), out object item))
            {
                if (typeof(T).IsSubclassOf(typeof(MonoBehaviour)))
                {
                    GameObject inst = new GameObject();
                    item      = inst.AddComponent(typeof(T)) as T;
                    inst.name = $"{typeof(T).Name}-Singleton";
                }
                else
                {
                    item = Activator.CreateInstance<T>();
                }

                m_class[typeof(T)] = item;
            }

            return item as T;
        }

        public static T Set<T>(T inst) where T : class
        {
            if (!Instance.m_class.ContainsKey(inst.GetType()))
            {
                Instance.m_class[inst.GetType()] = inst;
            }

            return inst;
        }

        private T Resolve<T>() where T : class
        {
            object result = m_class.TryGetValue(typeof(T), out object item) ? item : Set<T>();

            return result as T;
        }

        #region Camera

        private IDictionary<CameraInstance.CameraKey, CameraInstance> m_cameraMap =
            new Dictionary<CameraInstance.CameraKey, CameraInstance>();

        public static CameraInstance GetCamera(string sceneName, CameraInstance.CameraType type = CameraInstance.CameraType.SideCamera)
        {
            CameraInstance.CameraKey searchKey = new CameraInstance.CameraKey(type, sceneName);
            if (Instance.m_cameraMap.TryGetValue(searchKey, out CameraInstance instance))
            {
                return instance;
            }

            return null;
        }

        public static void SetCamera(CameraInstance camInstance, string sceneName, CameraInstance.CameraType type = CameraInstance.CameraType.SideCamera)
        {
            CameraInstance.CameraKey searchKey = new CameraInstance.CameraKey(type, sceneName);
            if (!Instance.m_cameraMap.TryGetValue(searchKey, out CameraInstance instance))
            {
                Instance.m_cameraMap[searchKey] = camInstance;
            }
        }

        #endregion

        public static IList<T> GetAll<T>() where T : class
        {
            IList<T> types = new List<T>();
            foreach (var service in Instance.m_class)
            {
                Type t = service.Key;
                object ins = service.Value;
                if (ins is T obj)
                {
                    types.AddIfNotContains(obj);
                }
            }

            return types;
        }

    }
}

