#region Header
// Date: 31/05/2023
// Created by: Huynh Phong Tran
// File name: BaseUI.cs
#endregion

using UnityEngine;
using UnityEngine.EventSystems;

namespace Base.Helper
{
    public class BaseUI : UIBehaviour
    {
        private RectTransform _rectTransform;
        private Transform _transform;
        private GameObject _gameObject;
        public int InstanceId => CacheGameObject.GetInstanceID();

        public Transform CacheTransform
        {
            get
            {
                if (_transform is null)
                {
                    _transform = transform;
                }

                return _transform;
            }
        }

        public GameObject CacheGameObject
        {
            get
            {
                if (_gameObject is null)
                {
                    _gameObject = gameObject;
                }

                return _gameObject;
            }
        }

        public RectTransform CacheRectTransform
        {
            get
            {
                if (_rectTransform is null)
                {
                    _rectTransform = gameObject.GetComponent<RectTransform>();
                }

                return _rectTransform;
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
    }
}