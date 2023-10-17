using System;
using System.Threading;
using Base.Logging;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Base
{
    public partial class AddressableManager
    {
        public AsyncOperationHandle<T> LoadAsset<T>(object key)
        {
            return Addressables.LoadAssetAsync<T>(key);
        }

        public AsyncOperationHandle<T>? LoadAssetAsync<T>(object key, Action<T> callback, int retryCount = 0, int retry = 0) where T : Object
        {
            try
            {
                AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);
                handle.Completed += OnCompleted;

                return handle;
            }
            catch (Exception e)
            {
                CheckThenRetry(null, e);

                return null;
            }

            void OnCompleted(AsyncOperationHandle<T> result)
            {
                if (result.Status == AsyncOperationStatus.Succeeded)
                {
                    CallInMainThread(result.Result);
                }
                else
                {
                    CheckThenRetry(result.Result, result.OperationException);
                }
            }

            void CallInMainThread(T obj)
            {
                AddTrackAsset(obj);
                callback?.Invoke(obj);
            }

            void CheckThenRetry(T obj, Exception e)
            {
                if (retry >= retryCount)
                {
                    AddTrackAsset(obj);
                    PDebug.Error($"[AddressableManager]LoadAssetAsync '{key}'. Error: '{GetError(e)}'");
                    callback?.Invoke(obj);
                }
                else
                {
                    retry++;

                    void CallRetry()
                    {
                        PDebug.Error($"[AddressableManager]LoadAssetAsync retry '{key}'");
                        LoadAssetAsync(key, callback, retryCount, retry);
                    }

                    Dispatch(action: CallRetry, delay: RETRY_DELAY_TIMER);
                }
            }
        }

        public async UniTask<T> LoadAsset<T>(object key, int retryCount = 0, int retry = 0, CancellationToken cancellationToken = default)
            where T : Object
        {
            try
            {
                return await Addressables.LoadAssetAsync<T>(key).ToUniTask(cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException canceledException)
            {
                return null;
            }
            catch (Exception exception)
            {
                if (retry >= retryCount)
                {
                    PDebug.GetLogger().Error("[AddressableManager] LoadAsset {asset} Error {error}", key, exception.Message);

                    return null;
                }
                else
                {
                    retry++;
                    await UniTask.Delay(TimeSpan.FromSeconds(RETRY_DELAY_TIMER), ignoreTimeScale: true, cancellationToken: cancellationToken);
                    PDebug.GetLogger().Error("[AddressableManager] LoadAsset {asset} Retry ... {count}", key, retry);

                    return await LoadAsset<T>(key, retryCount, retry, cancellationToken: cancellationToken);
                }
            }
        }

        public async UniTask<GameObject> InstantiateAsync(object key, Transform parent = null, bool instantiateInWorld = false, int retryCount = 0,
            int retry = 0, CancellationToken cancellationToken = default)
        {
            try
            {
                return await Addressables.InstantiateAsync(key, parent, instantiateInWorld).ToUniTask(cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException canceledException)
            {
                return null;
            }
            catch (Exception exception)
            {
                if (retry >= retryCount)
                {
                    PDebug.GetLogger().Error("[AddressableManager] Instantiate Object {obj} Error {error}", key, exception.Message);

                    return null;
                }

                retry++;
                await UniTask.Delay(TimeSpan.FromSeconds(RETRY_DELAY_TIMER), true, cancellationToken: cancellationToken);
                PDebug.GetLogger().Info("[AddressableManager] Retry Count {count}", retry);

                return await InstantiateAsync(key, parent, instantiateInWorld, retryCount, retry, cancellationToken);
            }
        }

        public AsyncOperationHandle<GameObject>? InstantiateAsync(object key, Action<GameObject> callback = null, Transform parent = null,
            bool instantiateInWorld = false, int retryCount = 0, int retry = 0)
        {
            try
            {
                AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(key, parent, instantiateInWorld);
                handle.Completed += OnCompleted;

                return handle;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                return null;
            }

            void OnCompleted(AsyncOperationHandle<GameObject> result)
            {
                if (result.Status == AsyncOperationStatus.Succeeded)
                {
                    CallInMainThread(result.Result);
                }
                else
                {
                    CheckThenRetry(result.Result, result.OperationException);
                }
            }

            void CallInMainThread(GameObject obj)
            {
                AddTrackInstance(obj);
                callback?.Invoke(obj);
            }

            void CheckThenRetry(GameObject obj, Exception e)
            {
                if (retry >= retryCount)
                {
                    AddTrackInstance(obj);
                    PDebug.Error($"[AddressableManager]InstantiateAsync '{key}'. Error: '{GetError(e)}'");
                    callback?.Invoke(obj);
                }
                else
                {
                    retry++;

                    void CallRetry()
                    {
                        PDebug.Error($"[AddressableManager]InstantiateAsync retry '{key}'");
                        LoadAssetAsync(key, callback, retryCount, retry);
                    }

                    Dispatch(action: CallRetry, delay: RETRY_DELAY_TIMER);
                }
            }
        }
    }
}

