using Base.Helper;
using UnityEngine;

namespace Base
{
    public class CameraFollow3Rd : BaseMono
    {
        [SerializeField] protected Transform followTarget;
        [SerializeField] protected Vector3 offset;
        [SerializeField] protected float smoothSpeed = .125f;

        public void InitOffset()
        {
            offset = followTarget.position - Position;
        }

        public void FollowTarget()
        {
            Vector3 desiredPos = followTarget.position - offset;
            Vector3 smoothPos = Vector3.Lerp(Position, desiredPos, smoothSpeed);
            Position.Set(smoothPos.x, Position.y, smoothPos.z);
        }
    }
}

