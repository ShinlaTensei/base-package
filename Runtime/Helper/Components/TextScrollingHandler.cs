using System;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Base.Helper
{
    [DisallowMultipleComponent]
    public class TextScrollingHandler : BaseUI
    {
        [SerializeField] private TMP_Text m_text;
        [SerializeField] private float m_speed;
        [SerializeField] private float m_extend = 15f;
        
        private bool m_isLengthInitialized = false;
        private float m_textLength;
        private float m_parentLength;
        private float m_lengthDiff;
        private float m_initialLengthDiff;
        private float m_direction;
        private float m_step;
        private Vector2 m_initialAnchorPos;

        protected override void Awake()
        {
            if (!m_text) throw new NullReferenceException();
            m_text.rectTransform.pivot = new Vector2(0, 0.5f);
            m_text.rectTransform.anchoredPosition = Vector2.zero;
        }
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/UI/BaseFramework/Text Scrolling")]
        public static void CreateTextScrollingGameObject(UnityEditor.MenuCommand menuCommand)
        {
            GameObject selectionGo = UnityEditor.Selection.activeGameObject;
            TextScrollingHandler textScrollingGo = new GameObject("Text Scrolling").AddComponent<TextScrollingHandler>();
            textScrollingGo.AddComponent<RectTransform>();
            if (selectionGo)
            {
                textScrollingGo.CacheRectTransform.SetParent(selectionGo.transform);
            }
            textScrollingGo.CacheRectTransform.anchoredPosition = Vector2.zero;
            var tmp = new GameObject("Text").AddComponent<TextMeshProUGUI>();
            tmp.AddComponent<ContentSizeFitter>();
            tmp.rectTransform.SetParent(textScrollingGo.CacheRectTransform);
            tmp.rectTransform.pivot = new Vector2(0, 0.5f);
            tmp.rectTransform.anchorMin = new Vector2(0, 0.5f);
            tmp.rectTransform.anchorMax = new Vector2(0, 0.5f);
            tmp.rectTransform.anchoredPosition = Vector2.zero;
            var member = textScrollingGo.GetType().GetField("m_text", BindingFlags.NonPublic | BindingFlags.Instance);
            if (member != null)
            {
                member.SetValue(textScrollingGo, (TMP_Text)tmp);
            }
        }
#endif
    }
}