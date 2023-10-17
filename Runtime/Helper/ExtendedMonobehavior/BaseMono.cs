using System;
using Base.Logging;
using NaughtyAttributes;
using UnityEngine;

namespace Base.Helper
{
    public class BaseMono : MonoBehaviour
    {
        [SerializeField] [ReadOnly] private bool isMissingReference;
        
        
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

        public bool IsMissingReference => isMissingReference;

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

        // protected virtual void OnValidate()
        // {
        //     isMissingReference = this.CheckNullAllSerializedFields();
        // }

        protected virtual void Start()
        {
            //if (isMissingReference) return;
        }

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
    }
}

