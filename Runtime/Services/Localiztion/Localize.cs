using System.Collections.Generic;
using Base.Pattern;
using UnityEngine;
using System;
using System.Text;
using Base.Logging;

namespace Base.Services
{
    public class LanguageChangedRequestSignal : Signal<string> {}

    public enum LanguageCode {En, Vi}
    
    public interface IBlueprintLocalization
    {
        public string GetTextByKey(string key);
    }
    
    public class LocalizeService : IService
    {
        private readonly Dictionary<SystemLanguage, LanguageCode> _supportLanguage = new Dictionary<SystemLanguage, LanguageCode>
        {
            {SystemLanguage.English, LanguageCode.En},
            {SystemLanguage.Vietnamese, LanguageCode.Vi}
        };

        private string KeyLang = "paidrubik_lang";

        private LanguageCode _currentLang;

        public LanguageCode CurrentLanguage => _currentLang;
        
        public void Init()
        {

            if (PlayerPrefs.HasKey(KeyLang))
            {
                if (Enum.TryParse(PlayerPrefs.GetString(KeyLang), out LanguageCode langCode))
                {
                    _currentLang = langCode;
                }
            }
            else if (_supportLanguage.ContainsKey(Application.systemLanguage))
            {
                _supportLanguage.TryGetValue(Application.systemLanguage, out _currentLang);
            }
            else
            {
                _currentLang = LanguageCode.Vi;
            }
        }

        public void DeInit()
        {
            PlayerPrefs.SetString(KeyLang, _currentLang.ToString());
            PlayerPrefs.Save();
        }

        public void SetLanguage(LanguageCode langCode)
        {
            bool isChanged = _currentLang != langCode;
            _currentLang = langCode;
            
            if (isChanged)
            {
                PlayerPrefs.SetString(KeyLang, _currentLang.ToString());
                PlayerPrefs.Save();

                try
                {
                    ServiceLocator.Get<LanguageChangedRequestSignal>()?.Dispatch(_currentLang.ToString());
                }
                catch (Exception e)
                {
                    PDebug.GetLogger().Error(e);
                }
            }
        }

        public void Dispose()
        {
        }
    }

    public static class Localization
    {
        private static string GetLocalizeText(string key)
        {
            IBlueprintLocalization blueprint = ServiceLocator.Get<IBlueprintLocalization>();
            string                 text      = blueprint?.GetTextByKey(key) ?? string.Empty;
            
            return !string.IsNullOrEmpty(text) ? text : key;
        }
        
        public static string GetText(string textID)
        {
            return GetLocalizeText(textID);
        }
        
        public static string Format(string textID, params object[] args)
        {
            return Format(null, GetLocalizeText(textID), args);
        }

        static string Format(IFormatProvider provider, string format, params object[] args)
        {
            if (format == null) { return null; }
            if (args == null) { throw new ArgumentNullException("args"); }

            StringBuilder sb = new StringBuilder(format.Length + args.Length * 8);
            sb.AppendFormat(provider, format, args);
            return sb.ToString();
        }
    }
}