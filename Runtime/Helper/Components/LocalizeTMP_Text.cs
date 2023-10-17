using System;
using Base.Pattern;
using Base.Services;
using TMPro;
using UnityEngine;

namespace Base.Helper
{
    public class LocalizeTMP_Text : BaseMono
    {
        [SerializeField] private string mainKey;
        private TMP_Text _tmpText;
        private bool _isInit = false;

        private void Awake()
        {
            _tmpText = GetComponent<TMP_Text>();
            ServiceLocator.Get<LanguageChangedRequestSignal>()?.Subscribe(OnLanguageChanged);
        }

        protected override void Start()
        {
            base.Start();
            
            if (!_isInit) SetText();
        }

        private void OnDestroy()
        {
            ServiceLocator.Get<LanguageChangedRequestSignal>()?.UnSubscribe(OnLanguageChanged);
        }

        private void OnLanguageChanged(string langCode)
        {
            SetText();
        }

        private void SetText()
        {
            _tmpText.text = Localization.GetText(mainKey);
            _isInit = true;
        }
    }
}