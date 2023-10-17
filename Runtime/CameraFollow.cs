using UnityEngine;
using Base.Helper;

namespace Base
{
    public class CameraFollow : BaseMono
    {
        [SerializeField] protected Transform followTarget;
        [SerializeField] protected Vector3 offset;
        [SerializeField] protected float smoothSpeed = .125f;

        private void Start()
        {
            InitOffset();
        }

        private void FixedUpdate()
        {
            FollowTarget();
        }

        public void InitOffset()
        {
            offset = followTarget.position - Position;
        }

        public void FollowTarget()
        {
            if (offset != Vector3.zero)
            {
                Vector3 desiredPos = followTarget.position - offset;
                Vector3 smoothPos = Vector3.Lerp(Position, desiredPos, smoothSpeed * Time.fixedDeltaTime);
                Position = new Vector3(smoothPos.x, Position.y, smoothPos.z);
            }
        }
    }
}

