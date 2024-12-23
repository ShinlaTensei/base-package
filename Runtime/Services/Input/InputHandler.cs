﻿using System.Collections.Generic;
using Base.Core;
using UnityEngine;

namespace Base
{
    public class InputHandler : Service
    {
        #if UNITY_EDITOR || UNITY_STANDALONE
        private bool m_isTouch = false;
        #elif UNITY_ANDROID
        private bool m_isTouch = true;
        #endif

        private List<Touch> m_touches = new List<Touch>();

        public List<Touch> GetTouch()
        {
            if (m_touches == null)
            {
                m_touches = new List<Touch>();
            }
            m_touches.Clear();
            if (m_isTouch)
            {
                m_touches.AddRange(Input.touches);

                return m_touches;
            }
            
            Touch fakeTouch = new Touch();
            if (Input.GetMouseButtonDown(0))
            {
                fakeTouch.phase         = TouchPhase.Began;
                fakeTouch.position      = Input.mousePosition;
                fakeTouch.deltaPosition = new Vector2(0, 0);
                m_touches.Add(fakeTouch);
            }
            else if (Input.GetMouseButton(0))
            {
                fakeTouch.phase         = TouchPhase.Moved;
                fakeTouch.position      = Input.mousePosition;
                fakeTouch.deltaPosition = Vector2.zero;
                m_touches.Add(fakeTouch);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                fakeTouch.phase         = TouchPhase.Ended;
                fakeTouch.position      = Input.mousePosition;
                fakeTouch.deltaPosition = Vector2.zero;
            }

            return m_touches;
        }

        public bool GetTouchBegan()
        {
            if (m_isTouch)
            {
                if (Input.touchCount == 0) return false;

                Touch touch = Input.touches[0];
                if (touch.phase is TouchPhase.Began)
                {
                    return true;
                }

                return false;
            }

            if (Input.GetMouseButtonDown(0))
            {
                return true;
            }

            return false;
        }
    }
}

