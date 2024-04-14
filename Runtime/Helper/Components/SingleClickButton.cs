

using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Base.Helper
{
    [AddComponentMenu("Base Component/Single Click Button")]
    public class SingleClickButton : Selectable, IPointerClickHandler, ISubmitHandler
    {
        private bool m_isClick = false;

        private Action m_onClick;

        protected SingleClickButton() : base() {}

        private void Press()
        {
            if (!IsActive() || !IsInteractable())
                return;
            
            if (m_isClick) return;
            m_isClick = true;

            UISystemProfilerApi.AddMarker("Button.onClick", this);
            m_onClick.Invoke();
        }
        
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            Press();
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            Press();

            // if we get set disabled during the press
            // don't run the coroutine.
            if (!IsActive() || !IsInteractable())
                return;

            DoStateTransition(SelectionState.Pressed, false);
            StartCoroutine(OnFinishSubmit());
        }

        public void Subscribe(Action func) => m_onClick += func;
        public void UnSubscribe(Action func) => m_onClick -= func;
        public void Completed(bool value) => m_isClick = !value;

        private IEnumerator OnFinishSubmit()
        {
            var fadeTime = colors.fadeDuration;
            var elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            DoStateTransition(currentSelectionState, false);
        }
    }
}