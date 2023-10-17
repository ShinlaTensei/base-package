using System.Threading;
using Base.Pattern;
using Base.Utilities;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Base.Helper
{
    public enum ExitType {None, Hide, Remove, RemoveImmediate}
    public enum NavigationState {
        None = 0,
        Obscured,
        Overlap}
    
    public enum ViewBackgroundType {None, Blur, Transparent}
    
    public interface IViewData {}
    public abstract class UIView : BaseUI, IPointerClickHandler
    {
        private const string RootName = "Root";
        [SerializeField] protected string       m_viewId;
        [SerializeField] protected GameObject   m_root;
        [SerializeField] protected ExitType     m_exitType;
        [SerializeField] protected UICanvasType m_canvasType;
        [BitFlag(typeof(NavigationState))]
        [SerializeField] protected long         m_navigationState = 0;

        [SerializeField] protected ViewBackgroundType m_backgroundType;
        [SerializeField] protected bool               m_activeDefault;
        [SerializeField] protected bool               m_closeOnTouchOutside;
        [SerializeField] protected bool               m_closePrevOnShow;
        [SerializeField] protected bool               m_triggerViewChange;
        [Condition("m_closeOnTouchOutside", true, false)] 
        [SerializeField] protected RectTransform  m_touchRect;
        
        [Header("Animation")]

        protected UIViewManager m_manager = null;

        protected IViewData m_data = null;

        protected bool m_isShow = false;

        public string ViewID => m_viewId;
        public GameObject Root
        {
            get
            {
                if (!m_root)
                {
                    m_root = CacheTransform.FindChildRecursive<GameObject>(RootName);
                }
                return m_root;
            }
        }

        public UIViewManager UIManager
        {
            get
            {
                if (m_manager == null) m_manager = ServiceLocator.Get<UIViewManager>();

                return m_manager;
            }
        }
        public ExitType           ExitType          => m_exitType;
        public UICanvasType       CanvasType        => m_canvasType;
        public bool               ActiveDefault     => m_activeDefault;
        public bool               ClosePrevOnShow   => m_closePrevOnShow;
        public bool               TriggerViewChange => m_triggerViewChange;
        public long               NavigationState   => m_navigationState;
        public ViewBackgroundType BackgroundType    => m_backgroundType;
        public bool               IsShowing         { get => m_isShow; set => m_isShow = value; }

        public virtual void Show()
        {
            //if (IsMissingReference) return;
            
            Root.SetActive(true);
            IsShowing = true;
        }

        public virtual void Hide()
        {
            //if (IsMissingReference) return;
            IsShowing = false;
            switch (m_exitType)
            {
                case ExitType.Hide:
                    Root.SetActive(false);
                    break;
                case ExitType.Remove:
                    Destroy(CacheGameObject);
                    break;
                case ExitType.RemoveImmediate:
                    DestroyImmediate(CacheGameObject);
                    break;
                default: break;
            }
        }
        
        public virtual void Next() {}

        public virtual void Back()
        {
            if (UIManager && UIManager.Previous)
            {
                UIManager.Show(UIManager.Previous).Forget();
            }
        }

        public virtual void Show<T>(T argument) where T : IViewData
        {
            Show();
            
            Populate(argument);
        }

        public virtual void Hide<T>(T argument) where T : IViewData
        {
            Hide();
        }

        public virtual void InternalClose()
        {
            if (UIManager != null)
            {
                UIManager.CloseView(this);
            }
            else
            {
                Hide();
            }
        }
        
        /// <summary>
        /// Wait for something on transition of UI
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>UniTask</returns>
        public virtual async UniTask Await(CancellationToken cancellationToken = default)
        {
            if (false)
            {
                await UniTask.Yield();
            }
        }

        public abstract void Populate<T>(T viewData) where T : IViewData;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!m_closeOnTouchOutside) return;
            bool inside = false;
            if (m_touchRect)
            {
                inside = RectTransformUtility.RectangleContainsScreenPoint(m_touchRect, eventData.position, eventData.pressEventCamera);
            }
            
            if(!inside) {Hide();}
        }
    }
}

