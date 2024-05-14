using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Base.Core;
using Base.Helper;
using Base.Logging;
using Base.Pattern;
using Base.Core;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Base.Core
{
    public enum UICanvasType
    {
        None             = 0,
        RootCanvas       = 1,
        ViewCanvas       = 2,
        TopViewCanvas    = 3,
        OverlayCanvas    = 4,
        RetryCanvasUI    = 5,
        UIOverlayLayout  = 6,
        CampaignCanvasUI = 7,
    }

    public class OnUIViewChangedSignal : Signal<UIView, UIView>
    {
    }

    public class UIViewManager : BaseMono
    {
        [SerializeField] private GameObject m_blurObj;
        [SerializeField] private GameObject m_transparentObj;
        private const            string     RootName = "Root";

        public bool IsInitialize { get; private set; } = false;

        #region UIView Handle

        private Dictionary<string, UIView> m_uiViewPool = new Dictionary<string, UIView>();
        private Stack<string>              m_stackUI = new Stack<string>();

        private UIView m_previous;
        private UIView m_current;

        private AddressableManager m_addressableManager;

        public UIView Previous => m_previous;

        protected override void Start()
        {
            base.Start();

            m_addressableManager = ServiceLocator.Get<AddressableManager>();
            RegisterContext(0);
            IsInitialize = true;
        }

        private void OnDestroy()
        {
            UnRegisterContext(0);
            IsInitialize = false;
        }

        /// <summary>
        /// Fire a signal when UI changed state
        /// </summary>
        private void NotifyUIViewChanged()
        {
            ServiceLocator.Get<OnUIViewChangedSignal>()?.Dispatch(m_previous, m_current);
        }
        private async UniTask<T> ShowAsync<T>(T instance, Action<T> onInit = null, Transform root = null, 
                                              CancellationToken cancellationToken = default) where T : UIView
        {
            T view = instance ? instance : GetView<T>();

            if (!view)
            {
                view = await InitAsync<T>(null, cancellationToken);
            }

            if (view)
            {
                await InitCompleted(view, root, cancellationToken);
            }

            onInit?.Invoke(view);

            return view;
        }

        private async UniTask<UIView> ShowAsync(string viewId, Action<UIView> onInit = null, Transform root = null,
                                                CancellationToken cancellationToken = default)
        {
            UIView instance = GetView(viewId);

            if (!instance)
            {
                instance = await InitAsync(viewId, null, cancellationToken);
            }

            if (instance)
            {
                await InitCompleted(instance, root, cancellationToken);
            }
            
            onInit?.Invoke(instance);

            return instance;
        }

        public async UniTask<T> Show<T>(T instance, IViewData viewData = null, Action<T> onInit = null, Transform root = null,
                                        CancellationToken cancellationToken = default) where T : UIView
        {
            T inst = await ShowAsync<T>(instance, onInit, root, cancellationToken).AttachExternalCancellation(cancellationToken);
            inst.Populate(viewData);
            return inst;
        }
        
        public async UniTask<T> Show<T>(IViewData viewData = null, Action<T> onInit = null, Transform root = null,
                                        CancellationToken cancellationToken = default) where T : UIView
        {
            T inst = await ShowAsync<T>(null, onInit, root, cancellationToken).AttachExternalCancellation(cancellationToken);
            inst.Populate(viewData);
            return inst;
        }

        public async UniTask<T> Show<T>(Action<T> onInit = null, Transform root = null, CancellationToken cancellationToken = default)
                where T : UIView
        {
            T inst = await ShowAsync<T>(null, onInit, root, cancellationToken).AttachExternalCancellation(cancellationToken);
            
            return inst;
        }

        public async UniTask<UIView> Show(string viewId, IViewData viewData = null, Action<UIView> onInit = null, Transform root = null,
                                             CancellationToken cancellationToken = default)
        {
            UIView view = await ShowAsync(viewId, onInit, root, cancellationToken);
            view.Populate(viewData);
            return view;
        }

        private void CloseView<T>(T view) where T : UIView
        {
            if (view is null) return;

            if (m_uiViewPool.ContainsValue(view))
            {
                view.OnHide();
                if (view.ExitType is ExitType.Remove or ExitType.RemoveImmediate)
                {
                    Remove(view);
                }

                view.Hide();
            }
        }

        public void CloseView<T>(T view, bool showPrevious, IViewData viewData = null) where T : UIView
        {
            string viewName = view.GetType().Name;
            CloseView(view);
            if (!showPrevious) return;

            string viewFromStack = GetViewFromStack();

            if (!string.IsNullOrEmpty(viewFromStack))
            {
                TaskRunner.Start(Show(viewFromStack, viewData).AsTask());
            }
        }

        public void Add<T>(T view) where T : UIView
        {
            if (!m_uiViewPool.ContainsKey(view.GetType().Name))
            {
                m_uiViewPool.TryAdd(view.GetType().Name, view);
            }
        }

        public void Remove<T>(T value) where T : UIView
        {
            if (m_uiViewPool.ContainsKey(value.GetType().Name))
            {
                m_uiViewPool.Remove(value.GetType().Name);
            }
        }

        public bool HasView<T>() where T : UIView
        {
            return GetView<T>() != null;
        }

        public T GetView<T>() where T : UIView
        {
            m_uiViewPool.TryGetValue(typeof(T).Name, out UIView value);

            return value as T;
        }
        
        public UIView GetView(string viewID)
        {
            m_uiViewPool.TryGetValue(viewID, out UIView value);

            return value;
        }

        private string GetViewFromStack()
        {
            string modelName = string.Empty;
            try
            {
                modelName = m_stackUI.Pop();
            }
            catch (Exception e)
            {
                PDebug.Error(e, "[UIView] Get View From Stack Failed: {0}", e.Message);
            }

            return modelName;
        }

        private void AddViewToStack<T>(T view) where T : UIView
        {
            UIModelAttribute attribute = view.GetType().GetCustomAttribute(typeof(UIModelAttribute), false) as UIModelAttribute;

            if (attribute == null)
            {
                PDebug.ErrorFormat("[UIView]Need to apply UIModelAttribute on class {name}", typeof(T).Name);

                return;
            }

            string modelName = attribute.ModelName;
            if (!m_stackUI.Contains(modelName))
            {
                m_stackUI.Push(modelName);
            }
        }

        private async UniTask<T> InitAsync<T>(Action<T> onCompleted = null, CancellationToken cancellationToken = default) where T : UIView
        {
            UIModelAttribute attribute = Attribute.GetCustomAttribute(typeof(T), typeof(UIModelAttribute)) as UIModelAttribute;

            if (attribute == null)
            {
                PDebug.ErrorFormat("[UIView]Need to apply UIModelAttribute on class {name}", typeof(T).Name);

                return null;
            }

            string modelName = attribute.ModelName;

            GameObject inst       = null;
            string     prefabPath = string.Empty;

            if (m_addressableManager.IsInitialize && m_addressableManager.IsReadyToGetBundle)
            {
                prefabPath = modelName;
                inst = await m_addressableManager.InstantiateAsync(prefabPath,
                        parent: GetCanvasWithTag(UICanvasType.RootCanvas, attribute.SceneName), retryCount: 5,
                        cancellationToken: cancellationToken);
            }

            T view = inst != null ? inst.GetComponent<T>() : null;
            onCompleted?.Invoke(view);

            return view;
        }

        private async UniTask<UIView> InitAsync(string viewID, Action<UIView> onCompleted = null, 
                                               CancellationToken cancellationToken = default)
        {
            string modelName = viewID;

            GameObject inst       = null;
            string     prefabPath = string.Empty;

            if (m_addressableManager.IsInitialize && m_addressableManager.IsReadyToGetBundle)
            {
                prefabPath = modelName;
                inst = await m_addressableManager.InstantiateAsync(prefabPath, retryCount: 5,
                        cancellationToken: cancellationToken);
            }

            UIView view = inst != null ? inst.GetComponent<UIView>() : null;
            onCompleted?.Invoke(view);

            return view;
        }

        private async UniTask InitCompleted<T>(T instance, Transform root = null, CancellationToken cancellationToken = default)
                where T : UIView
        {
            if (instance == null)
            {
                PDebug.ErrorFormat("[UIView]Null reference of type {type}", typeof(T).Name);

                return;
            }

            Transform parent = root != null ? root : GetCanvasWithTag(instance.CanvasType, instance.CacheGameObject.scene.name);
            if (parent) instance.CacheTransform.SetParent(parent, false);

            instance.CacheTransform.SetScale(1);
            instance.CacheTransform.SetLocalPosition(Vector3.zero);
            instance.CacheRectTransform.anchoredPosition = Vector3.zero;
            if (instance.NavigationState.HasBit(NavigationState.Overlap))
            {
                instance.CacheTransform.SetAsLastSibling();
            }
            else if (instance.NavigationState.HasBit(NavigationState.Obscured))
            {
                instance.CacheTransform.SetAsFirstSibling();
            }

            instance.Root.SetActive(instance.ActiveDefault);

            if (instance.BackgroundType is ViewBackgroundType.Blur)
            {
                GameObject blurObj = Instantiate(m_blurObj, instance.Root.transform);
                blurObj.transform.SetAsFirstSibling();
            }
            else if (instance.BackgroundType is ViewBackgroundType.Transparent)
            { 
                GameObject transparentObj = Instantiate(m_transparentObj, instance.Root.transform);
                transparentObj.transform.SetAsFirstSibling();
            }

            m_previous = m_current;
            m_current  = instance;
            m_current.OnShow();
            await m_current.Await(cancellationToken).AttachExternalCancellation(cancellationToken);
            m_current.Show();
            if (m_current.ClosePrevOnShow) CloseView(m_previous);

            PDebug.InfoFormat("[UIViewManager] Request show {0} - Previous {1}", m_current.name, m_previous ? m_previous.name : "NULL");
            
            Add(instance);
            if (m_current.TriggerViewChange)
            {
                AddViewToStack(m_current);
                NotifyUIViewChanged();
            }

        }

        #endregion

        #region Canvas Handle

        private Dictionary<string, Transform> m_uiCanvasPool = new Dictionary<string, Transform>();

        public Transform GetCanvasWithTag(UICanvasType enumTag, string sceneName)
        {
            string newTag = !(enumTag is UICanvasType.None) ? enumTag.ToString() : UICanvasType.RootCanvas.ToString();
            Scene  scene  = SceneManager.GetSceneByName(sceneName);

            if (!scene.isLoaded) return null;

            string key = $"{newTag}-{scene.name}";
            if (m_uiCanvasPool.TryGetValue(key, out Transform result))
            {
                return result;
            }

            GameObject[] objects = GameObject.FindGameObjectsWithTag(newTag);
            for (int i = 0; i < objects.Length; ++i)
            {
                if (objects[i].scene.name.Equals(scene.name, StringComparison.CurrentCulture))
                {
                    Transform final = objects[i].transform.FindChildRecursive<Transform>(RootName);
                    m_uiCanvasPool.TryAdd($"{newTag}-{scene.name}", final);

                    return final;
                }
            }

            PDebug.WarnFormat("Cannot find canvas with tag {tag} in scene: {sceneName}", newTag, sceneName);

            return null;
        }

        public void Remove(UICanvasType canvasType, string sceneName)
        {
            string key = $"{canvasType.ToString()}-{sceneName}";
            if (m_uiCanvasPool.ContainsKey(key))
            {
                m_uiCanvasPool.Remove(key);
            }
        }

        public void RegisterCanvas(CanvasRegister canvas)
        {
            string key = $"{canvas.CacheGameObject.tag}-{canvas.CacheGameObject.scene.name}";
            if (!m_uiCanvasPool.ContainsKey(key))
            {
                m_uiCanvasPool[key] = canvas.CacheTransform;
            }
        }

        #endregion
    }
}