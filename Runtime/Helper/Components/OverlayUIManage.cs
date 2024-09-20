using UnityEngine;

namespace Base.Helper
{ 
    namespace Game
    {
        public class OverlayUIManage : BaseMono
        {
            [SerializeField] protected Transform targetTransform;
            [SerializeField] protected Vector3 offset;

            protected Camera _mainCamera;

            protected virtual void Start()
            {
                _mainCamera = Camera.main;
            }

            protected virtual void LateUpdate()
            {
                if (targetTransform != null)
                {
                    Vector3 newPos = _mainCamera.WorldToScreenPoint(targetTransform.position + offset);
                    Position = newPos;
                }
            }

            public void SetTarget(Transform target, Vector3 value)
            {
                targetTransform = target;
                this.offset = value;
            }
        } 
    }
}