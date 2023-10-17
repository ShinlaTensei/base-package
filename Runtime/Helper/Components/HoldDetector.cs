using Base.Helper;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Base.Helper
{
    public class HoldDetector : BaseMono, IPointerDownHandler, IPointerUpHandler
    {
        /// <summary>
        /// The time determine a press action turn to hold
        /// </summary>
        [SerializeField] private float m_timePressToHold = .25f;
        
        private float   m_holdTime;
        private bool    m_isHold;
        private bool    m_isPress;

        private UnityEvent m_onLongPress = new UnityEvent();
        private UnityEvent m_clickEvent  = new UnityEvent();
    
        public UnityEvent OnLongPress       => m_onLongPress;
        public UnityEvent ClickEvent        => m_clickEvent;
        public bool       IsHold            => m_isHold;
        
    
    
        private void LateUpdate()
        {
            if (m_isPress && !m_isHold)
            {
                if (m_holdTime > 0)
                {
                    m_holdTime -= Time.smoothDeltaTime;
                }
                else
                {
                    m_isHold = true;
                    m_onLongPress?.Invoke();
                }
            }
        }
    
        public void OnPointerDown(PointerEventData eventData)
        {
            if (m_isHold)
            {
                m_isPress = false;
                m_isHold = false;
                m_clickEvent?.Invoke();
            }
            else
            {
                m_holdTime = m_timePressToHold;
                m_isPress = true;
                m_isHold = false;
            }
        }
    
        public void OnPointerUp(PointerEventData eventData)
        {
            m_isPress = false;
            m_holdTime = m_timePressToHold;
        }
    }
}

