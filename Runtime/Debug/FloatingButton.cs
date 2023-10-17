using System;
using System.Collections;
using System.Collections.Generic;
using Base.Helper;
using Base.Logging;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Base
{
    public class FloatingButton : BaseMono, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private float holdingThreshold = 30f;
        [SerializeField] private float defaultSize = 100f;
        [SerializeField] private RectTransform parent;
        [SerializeField] private Vector3 defaultPosition;
        
        private bool _onDrag = false;
        private Vector2 _startPosition;
        private float _scaleFactor;

        public Action OnClick { get; set; }

        private void Awake()
        {
            defaultSize = CacheRectTransform.rect.width;
            CacheRectTransform.anchoredPosition3D = defaultPosition;

            DockToHorizontalEdge();
        }

        private void DockToHorizontalEdge()
        {
            Vector3 position = CacheRectTransform.anchoredPosition3D;
            Vector3 newPos = position;

            float scale = RectTransformUtils.GetCanvasScaleOnWidth(Screen.width, parent.GetComponent<Canvas>());
            _scaleFactor = scale;
            float actualWidth = Screen.width / scale;
            bool right = newPos.x >= 0;
            newPos.x = right ? actualWidth - defaultSize: -actualWidth + defaultSize;
            newPos.x = newPos.x / 2;

            CacheRectTransform.anchoredPosition3D = newPos;
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            _startPosition = CacheRectTransform.anchoredPosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 p1 = eventData.pressPosition;
            Vector2 p2 = eventData.position;
            float distance = Mathf.Abs(p1.magnitude - p2.magnitude);
            if (distance >= holdingThreshold)
            {
                _onDrag = true;
                CacheRectTransform.anchoredPosition += eventData.delta / _scaleFactor;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            DockToHorizontalEdge();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _onDrag = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_onDrag)
            {
                OnClick?.Invoke();
            }

            _onDrag = false;
        }
    }
}

