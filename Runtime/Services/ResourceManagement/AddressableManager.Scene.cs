using System;
using System.Threading;
using Base.Logging;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Base
{
    public partial class AddressableManager
    {
        public AsyncOperationHandle<SceneInstance> LoadSceneAsync(object key, Action<SceneInstance> callback = null,
            LoadSceneMode mode = LoadSceneMode.Additive, bool activeOnLoad = false, int retryCount = 0, int retry = 0)
        {
            try
            {
                AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(key, mode, activeOnLoad);
                handle.Completed += OnCompleted;

                return handle;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                throw;
            }

            void OnCompleted(AsyncOperationHandle<SceneInstance> argument)
            {
                if (argument.Status == AsyncOperationStatus.Succeeded)
                {
                    callback?.Invoke(argument.Result);
                }
                else
                {
                    CheckThenRetry(argument, argument.OperationException);
                }
            }

            void CheckThenRetry(AsyncOperationHandle<SceneInstance> result, Exception e)
            {
                if (retry >= retryCount)
                {
                    PDebug.GetLogger().Error("[AddressableManager]LoadSceneAsync '{key}'. Error: '{error}'", key, e.Message);
                    callback?.Invoke(result.Result);
                }
                else
                {
                    retry++;

                    void CallRetry()
                    {
                        PDebug.GetLogger().Info("[AddressableManager]LoadSceneAsync retry '{key}'", key);
                        LoadSceneAsync(key, callback, mode, activeOnLoad, retryCount, retry);
                    }

                    Dispatch(action: CallRetry, delay: RETRY_DELAY_TIMER);
                }
            }
        }

        public async UniTask<SceneInstance?> LoadSceneAsync(object key, LoadSceneMode mode = LoadSceneMode.Additive, bool activeOnLoad = false,
            int retryCount = 0, int retry = 0, CancellationToken cancellationToken = default)
        {
            try
            {
                return await Addressables.LoadSceneAsync(key, mode, activeOnLoad).ToUniTask(cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException canceledException)
            {
                return null;
            }
            catch (Exception exception)
            {
                if (retry >= retryCount)
                {
                    PDebug.GetLogger().Error("[AddressableManager]LoadSceneAsync '{key}'. Error: '{error}'", key, exception.Message);

                    return null;
                }
                else
                {
                    retry++;
                    await UniTask.Delay(TimeSpan.FromSeconds(RETRY_DELAY_TIMER), ignoreTimeScale: true, cancellationToken: cancellationToken);
                    PDebug.GetLogger().Info("[AddressableManager]LoadSceneAsync retry {0} times", retry);

                    return await LoadSceneAsync(key, mode, activeOnLoad, retryCount, retry, cancellationToken);
                }
            }
        }

        public AsyncOperationHandle<SceneInstance>? UnloadScene(SceneInstance scene, Action<bool> callback, bool autoReleaseHandle = true)
        {
            void OnComplete(AsyncOperationHandle<SceneInstance> result)
            {
                if (result.Status == AsyncOperationStatus.Succeeded)
                {
                    CallInMainThread();
                }
                else
                {
                    CheckThenRetry(result.OperationException);
                }
            }

            void CallInMainThread()
            {
                callback?.Invoke(true);
            }

            void CheckThenRetry(Exception e)
            {
                PDebug.GetLogger().Error("[AddressableManager]UnloadScene Error: '{error}'", e.Message);
                callback?.Invoke(false);
            }

            try
            {
                AsyncOperationHandle<SceneInstance> handle = Addressables.UnloadSceneAsync(scene, autoReleaseHandle);
                handle.Completed += OnComplete;

                return handle;
            }
            catch (Exception e)
            {
                CheckThenRetry(e);
            }

            return null;
        }

        public async UniTask<SceneInstance?> UnloadScene(SceneInstance scene, bool autoReleaseHandle = true,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await Addressables.UnloadSceneAsync(scene, autoReleaseHandle).ToUniTask(cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException e)
            {
                return null;
            }
            catch (Exception e)
            {
                PDebug.GetLogger().Error("[AddressableManager]UnloadScene Error: '{e}'", e.Message);

                return null;
            }
        }

        public AsyncOperationHandle<SceneInstance>? UnloadScene(AsyncOperationHandle handle, Action<bool> callback,
            bool autoReleaseHandle = true)
        {
            void OnComplete(AsyncOperationHandle<SceneInstance> result)
            {
                if (result.Status == AsyncOperationStatus.Succeeded)
                {
                    CallInMainThread();
                }
                else
                {
                    CheckThenRetry(result.OperationException);
                }
            }

            void CallInMainThread()
            {
                callback?.Invoke(true);
            }

            void CheckThenRetry(Exception e)
            {
                PDebug.GetLogger().Error("[AddressableManager]UnloadScene Error: '{e}'", e.Message);
                callback?.Invoke(false);
            }

            try
            {
                AsyncOperationHandle<SceneInstance> ops = Addressables.UnloadSceneAsync(handle, autoReleaseHandle);
                ops.Completed += OnComplete;

                return ops;
            }
            catch (Exception e)
            {
                CheckThenRetry(e);
            }

            return null;
        }

        public async UniTask<SceneInstance?> UnloadScene(AsyncOperationHandle handle, bool autoReleaseHandle = true,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await Addressables.UnloadSceneAsync(handle, autoReleaseHandle).ToUniTask(cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException e)
            {
                return null;
            }
            catch (Exception e)
            {
                PDebug.GetLogger().Error("[AddressableManager]UnloadScene Error: '{e}'", e.Message);

                return null;
            }
        }

        public AsyncOperationHandle<SceneInstance>? UnloadScene(AsyncOperationHandle? handle, Action<bool> callback,
            bool autoReleaseHandle = true)
        {
            if (handle != null && handle.HasValue)
            {
                return UnloadScene(handle.Value, callback, autoReleaseHandle);
            }

            callback?.Invoke(true);

            return null;
        }

        public async UniTask<SceneInstance?> UnloadScene(AsyncOperationHandle? handle, bool autoReleaseHandle = true,
            CancellationToken cancellationToken = default)
        {
            if (handle != null && handle.HasValue)
            {
                return await UnloadScene(handle.Value, autoReleaseHandle, cancellationToken);
            }

            return null;
        }
    }
}

