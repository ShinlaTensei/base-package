using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Base.Logging;
using Base.Pattern;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.Exceptions;
using Object = UnityEngine.Object;

namespace Base
{
    public partial class AddressableManager : MonoBehaviour, IService
    {
        public static float RETRY_DELAY_TIMER = 2f;
        public bool IsInit { get; set; }
        public bool IsReadyToGetBundle { get; set; }

        #region Tracking Asset

        private object _lock = new object();
        private List<Object> _trackAssets = new List<Object>();
        private List<GameObject> _trackInstances = new List<GameObject>();

        /// <summary>
        /// Release Tracked Instance Bundles
        /// All the objects that loaded into memory can be released by calling this method
        /// </summary>
        public void ReleaseTracked()
        {
            lock (_lock)
            {
                for (int index = 0; index < _trackAssets.Count; index++)
                {
                    object o = _trackAssets[index];
                    if (o != null)
                    {
                        try
                        {
                            Addressables.Release(o);
                        }
                        catch
                        {
                        }
                    }
                }

                for (int index = 0; index < _trackInstances.Count; index++)
                {
                    GameObject o = _trackInstances[index];
                    if (o != null)
                    {
                        try
                        {
                            Addressables.ReleaseInstance(o);
                        }
                        catch
                        {
                        }
                    }
                }

                _trackInstances.Clear();
                _trackAssets.Clear();
            }
        }

        private void AddTrackAsset(Object obj)
        {
            lock (_lock)
            {
                if (obj != null) _trackAssets.Add(obj);
            }
        }

        private void AddTrackAssets(IEnumerable<Object> objects)
        {
            lock (_lock)
            {
                if (objects != null) _trackAssets.AddRange(objects);
            }
        }

        public void AddTrackInstance(GameObject obj)
        {
            lock (_lock)
            {
                if (obj) _trackInstances.Add(obj);
            }
        }

        public void AddTrackInstances(IEnumerable<GameObject> objs)
        {
            lock (_lock)
            {
                if (objs != null) _trackInstances.AddRange(objs);
            }
        }

        public static string GetError(Exception e)
        {
            while (e != null)
            {
                if (e is RemoteProviderException remoteException)
                {
                    return remoteException.WebRequestResult.Error;
                }

                if (e is InvalidKeyException keyException)
                {
                    return $"{keyException.Type.Name} not found key {keyException.Key}";
                }

                if (e.InnerException != null)
                {
                    e = e.InnerException;
                }
                else
                {
                    break;
                }
            }

            return e?.ToString();
        }
        #endregion
        
        #region Init & Update

        public void Init()
        {
            Initialize();
        }

        public void DeInit()
        {
            ClearAtlases();
            ReleaseTracked();
        }
        
        
        public void Dispose()
        {
            ClearAtlases();
            ReleaseTracked();
        }

        public void Initialize(Action<bool> callback = null, int retryCount = 0, int retry = 0)
        {
            PDebug.GetLogger().Info("[AddressableManager] Initializing ...");

            IsInit = false;
            IsReadyToGetBundle = false;

            try
            {
                Addressables.InitializeAsync().Completed += OnCompleted;
            }
            catch (Exception e)
            {
                Retry(e);
            }

            void OnCompleted(AsyncOperationHandle<IResourceLocator> handle)
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    CallOnMainThread();
                }
                else if (handle.Status == AsyncOperationStatus.Failed)
                {
                    Retry(handle.OperationException);
                }
            }

            void CallOnMainThread()
            {
                PDebug.Info("[AddressableManager] Initializing Completed!!!");

                IsInit = true;
                IsReadyToGetBundle = true;
                callback?.Invoke(true);
            }

            void Retry(Exception ex)
            {
                if (retry >= retryCount)
                {
                    PDebug.ErrorFormat("[AddressableManager] Initializing Error: {msg}", ex.Message);
                    IsInit = true;
                    IsReadyToGetBundle = false;
                    callback?.Invoke(false);
                }
                else
                {
                    retry++;

                    void CallRetry()
                    {
                        PDebug.Info("[AddressableManager]Initialized retry");
                        Initialize(callback, retryCount, retry);
                    }

                    Dispatch(action: CallRetry, delay: RETRY_DELAY_TIMER);
                }
            }
        }

        public async UniTask<bool> InitializeAsync(Action<bool> callback = null, int retryCount = 0, int retry = 0,
            CancellationToken cancellationToken = default)
        {
            PDebug.GetLogger().Info("[AddressableManager] Initializing ...");

            IsInit = false;
            IsReadyToGetBundle = false;

            try
            {
                await Addressables.InitializeAsync().ToUniTask(cancellationToken: cancellationToken);

                PDebug.GetLogger().Info("[AddressableManager] Initializing Completed!!!");
                IsInit = IsReadyToGetBundle = true;

                return true;
            }
            catch (OperationCanceledException canceledException)
            {
                return false;
            }
            catch (Exception exception)
            {
                if (retry >= retryCount)
                {
                    PDebug.GetLogger().Error("[AddressableManager] Initializing Error: {msg}", exception.Message);
                    IsInit = true;
                    IsReadyToGetBundle = false;
                    callback?.Invoke(false);

                    return false;
                }
                else
                {
                    retry++;
                    PDebug.GetLogger().Info("[AddressableManager] Initializing Retry");

                    return await InitializeAsync(callback, retryCount, retry, cancellationToken: cancellationToken);
                }
            }
        }

        #endregion

        public static void Dispatch(float delay, Action action)
        {
            UnityMainThreadDispatcher.Instance().Dispatch(delay, action, WaitForNetwork);
        }

        static IEnumerator WaitForNetwork()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                yield return new WaitUntil(() => Application.internetReachability != NetworkReachability.NotReachable);
            }
        }
        
        public AsyncOperationHandle<long>? GetDownloadSizeAsync(object key, Action<long> callback)
        {
            void OnComplete(AsyncOperationHandle<long> result)
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

            void CallInMainThread(long result)
            {
                callback?.Invoke(result);
            }
            void CheckThenRetry(long result, Exception e)
            {
                PDebug.ErrorFormat($"[AddressableManager]GetDownloadSize '{key}'. Error: '{GetError(e)}'");
                callback?.Invoke(result);
            }

            try
            {
                AsyncOperationHandle<long> handle = Addressables.GetDownloadSizeAsync(key);
                handle.Completed += OnComplete;
                return handle;
            }
            catch (Exception e)
            {
                CheckThenRetry(0, e);
                return null;
            }
        }
    }
}