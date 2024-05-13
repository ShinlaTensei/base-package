using System;
using Base.Helper;
using Base.Logging;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Base.Core
{
    public record DependencyObject
    {
        public Type Type { get; private set; }
        public object Target { get; private set; }
        public Object UnityObject { get; private set; }

        public DependencyObject(Type type, object target, Object unityObject)
        {
            Type = type;
            Target = target;
            UnityObject = unityObject;
        }

        public Transform GetTransform()
        {
            if (UnityObject == null)
            {
                PDebug.Info("[DependencyContext] The object has been destroyed, return null !!!");
                return null;
            }

            switch (UnityObject)
            {
                case BaseMono baseMono:
                    return baseMono.CacheTransform;
                case MonoBehaviour monoBehaviour: return monoBehaviour.transform;
                case Component component: return component.transform;
                default: return null;
            }
        }
    }
}