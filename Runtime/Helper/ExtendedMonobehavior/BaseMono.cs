using System;
using Base.Core;
using Base.Logging;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Base.Helper
{
    public class BaseMono : MonoBehaviour
    {
        private RectTransform m_rectTransform;
        private Transform m_transform;
        private GameObject m_gameObject;
        public int InstanceId => CacheGameObject.GetInstanceID();

        public Transform CacheTransform
        {
            get
            {
                if (m_transform == null)
                {
                    m_transform = transform;
                }

                return m_transform;
            }
        }

        public GameObject CacheGameObject
        {
            get
            {
                if (m_gameObject == null)
                {
                    m_gameObject = gameObject;
                }

                return m_gameObject;
            }
        }

        public RectTransform CacheRectTransform
        {
            get
            {
                if (m_rectTransform == null)
                {
                    m_rectTransform = gameObject.GetComponent<RectTransform>();
                }

                return m_rectTransform;
            }
        }

        public bool Active
        {
            get => CacheGameObject.activeSelf;
            set => CacheGameObject.SetActive(value);
        }

        public Transform Parent
        {
            get => CacheTransform.parent;
            set => CacheTransform.SetParent(value);
        }

        public Vector3 Position
        {
            get => CacheTransform.position;
            set => CacheTransform.position = value;
        }

        public Quaternion Rotation
        {
            get => CacheTransform.rotation;
            set => CacheTransform.rotation = value;
        }

        public Vector3 EulerAngles
        {
            get => CacheTransform.eulerAngles;
            set => CacheTransform.eulerAngles = value;
        }

        public Vector3 Scale
        {
            get => CacheTransform.localScale;
            set => CacheTransform.localScale = value;
        }

        protected virtual void Start() { }

        public GameObject CacheInstantiate()
        {
            var obj = Instantiate(CacheGameObject);
            return obj;
        }

        public GameObject CacheInstantiate(Vector3 pos, Quaternion rotate, Transform parent)
        {
            var obj = Instantiate(CacheGameObject, parent.TransformPoint(pos), rotate, parent);
            return obj;
        }
        
        public static Transform CacheInstantiate(Transform prefab, Vector3 pos, Quaternion rotate, Transform parent)
        {
            var obj = Instantiate(prefab, parent.TransformPoint(pos), rotate, parent);
            return obj;
        }
        
        public Component CacheInstantiate(Component prefab, Vector3 pos, Quaternion rotate, Transform parent)
        {
            var obj = Instantiate(prefab, parent.TransformPoint(pos), rotate, parent);
            return obj;
        }

        public void RegisterContext<T>(int contextId, T source) where T : MonoBehaviour
        {
            BaseContextRegistry.TryGetOrCreateContext(contextId).Register(source);
        }

        public void UnRegisterContext<T>(int contextId, T source) where T : MonoBehaviour
        {
            BaseContextRegistry.TryGetOrCreateContext(contextId)?.UnRegister(source);
        }
    }
}

