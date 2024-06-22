#region Header
// Date: 31/05/2023
// Created by: Huynh Phong Tran
// File name: BaseUI.cs
#endregion

using Base.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Base.Helper
{
    public class BaseUI : UIBehaviour
    {
        private RectTransform m_rectTransform;
        private Transform m_transform;
        private GameObject m_gameObject;
        public int InstanceId => CacheGameObject.GetInstanceID();

        public Transform CacheTransform
        {
            get
            {
                if (m_transform is null)
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
                if (m_gameObject is null)
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
                if (m_rectTransform is null)
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
        
        public virtual void Next() {}
        public virtual void Back() {}

        public virtual void Show()
        {
            Active = true;
        }

        public virtual void Hide()
        {
            Active = false;
        }
        
        public void RegisterContext<T>(int contextId, T source) where T : MonoBehaviour
        {
            BaseContextRegistry.TryGetOrCreateContext(contextId).Register(source);
        }

        public void UnRegisterContext<T>(int contextId, T source) where T : MonoBehaviour
        {
            BaseContextRegistry.TryGetOrCreateContext(contextId).UnRegister(source);
        }
    }
}