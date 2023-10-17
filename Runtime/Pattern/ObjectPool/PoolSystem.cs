
using System;
using System.Collections.Generic;
using Base.Helper;
using Base.Logging;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;
using UniRx.Toolkit;

namespace Base.Pattern
{
    public interface IPoolUnit
    {
        void Dispose();
    }
    public class PoolSystem : SingletonMono<PoolSystem>
    {
        private IDictionary<string, IPoolUnit> m_poolDictionary = new Dictionary<string, IPoolUnit>();

        private IDictionary<string, IPoolUnit> PoolDictionary => m_poolDictionary;

        private CompositeDisposable CompositeDisposable { get; set; } = new CompositeDisposable();
        
        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var poolUnit in m_poolDictionary)
            {
                poolUnit.Value.Dispose();
            }
            
            CompositeDisposable.Clear();
        }

        public static void CreatePool<T>(T instance) where T : Component
        {
            if (!IsPoolExist<T>())
            {
                PoolUnit<T> poolInstance = new PoolUnit<T>(instance);
                Instance.PoolDictionary.TryAdd(typeof(T).Name, poolInstance);
            }
        }

        private static bool IsPoolExist<T>() where T : Component
        {
            return Instance.PoolDictionary.ContainsKey(typeof(T).Name);
        }

        public static T Rent<T>(Vector3 initialPosition, Quaternion rotation, Transform parent = null, bool useWorldPos = false) where T : Component
        {
            if (Instance.PoolDictionary.TryGetValue(typeof(T).Name, out IPoolUnit poolUnit))
            {
                if (poolUnit is PoolUnit<T> pool) return pool.GetUnit(initialPosition, rotation, parent, useWorldPos);
            }

            return null;
        }

        public static void Return<T>(T instance) where T : Component
        {
            if (Instance.PoolDictionary.TryGetValue(typeof(T).Name, out IPoolUnit poolUnit))
            {
                if (poolUnit is PoolUnit<T> pool) pool.Return(instance);
            }
        }

        public static void Preload<T>(int preloadCount, int threshold) where T : Component
        {
            if (Instance.PoolDictionary.TryGetValue(typeof(T).Name, out IPoolUnit poolUnit))
            {
                if (poolUnit is PoolUnit<T> pool)
                {
                    IObservable<Unit> observable = pool.PreloadAsync(preloadCount, threshold);
                    observable.Subscribe(unit => PDebug.InfoFormat("[PoolSystem] Preload Complete of {0}", typeof(T).Name), Instance.OnPreloadError)
                              .AddTo(Instance.CompositeDisposable);
                }
            }
        }

        #region Callback

        void OnPreloadError(Exception exception)
        {
            #if UNITY_EDITOR
            Debug.Break();
            #endif
            switch (exception)
            {
                case NullReferenceException:
                case ArgumentNullException:
                default:
                    PDebug.Error(exception, "[PoolSystem] ERROR: {0}", exception.Message);
                    break;
            }
        }

        #endregion
        
    }

    public class PoolUnit<T> : ObjectPool<T>, IPoolUnit where T : Component
    {
        public PoolUnit(T prefab)
        {
            m_prefab = prefab;
        }
        
        private T m_prefab;

        protected override T CreateInstance()
        {
            return Object.Instantiate(m_prefab);
        }

        public T GetUnit(Vector3 initialPosition, Quaternion rotation, Transform parent = null, bool useWorldPos = false)
        {
            T instance = Rent();

            if (parent)
            {
                Transform transform = instance.transform;
                transform.SetParent(parent, useWorldPos);
                transform.position = initialPosition;
                transform.rotation = rotation;
            }
            else
            {
                Transform transform = instance.transform;
                transform.position = initialPosition;
                transform.rotation = rotation;
            }

            return instance;
        }

        protected override void OnBeforeReturn(T instance)
        {
            instance.transform.RemoveFromParent();
            instance.transform.position = Vector3.zero;
            instance.transform.rotation = Quaternion.identity;
            base.OnBeforeReturn(instance);
        }
    }
}

