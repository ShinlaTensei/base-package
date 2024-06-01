using System.Collections;
using System.Collections.Generic;
using Base.Pattern;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Base.Helper
{
    [AddComponentMenu("Base Component/Audio Event Listener")]
    public class AudioEventListener : BaseMono
    {
        [SerializeField, ValueDropdown(nameof(AddNewAudioEvent))] private List<AudioEventBase> m_audioEvents = new List<AudioEventBase>();

        public void PlayEvent(string eventName)
        {
            AudioEventBase audioEventBase = m_audioEvents.Find(a => a.EventName.Equals(eventName));

            if (audioEventBase != null)
            {
                ServiceLocator.Get<AudioService>().Play(audioEventBase);
            }
        }

        private IEnumerable AddNewAudioEvent()
        {
            ValueDropdownList<AudioEventBase> valueDropdownList = new ValueDropdownList<AudioEventBase>
            {
                { "AudioEventBase", new AudioEventBase() },
                { "OneShotAudioEvent", new OneShotAudioEvent() }
            };

            return valueDropdownList;
        }
    }
}