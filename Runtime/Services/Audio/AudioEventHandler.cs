#region Header
// Date: 31/12/2023
// Created by: Huynh Phong Tran
// File name: AudioEventHandler.cs
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Base.Services
{
    public class AudioEventHandler : SerializedMonoBehaviour
    {
        [SerializeField, ValueDropdown(nameof(GetAudioEventsValue), DrawDropdownForListElements = false), ListDrawerSettings(ShowFoldout = false)]
        private List<AudioEvent> m_audioEvents = new List<AudioEvent>();
        
        // This code run on editor
        private IEnumerable GetAudioEventsValue()
        {
            List<ValueDropdownItem> result = new List<ValueDropdownItem>()
                                             {
                                                     new("OneShot", new AudioEvent()),
                                                     new("Sound", new AudioEvent()),
                                                     new("Music", new AudioEvent())
                                             };

            return result;
        }
        // -----------------------------------------------------------------------------------------------------------------------
        private void Start()
        {
            AutomaticallyInvoke(InvokeStrategy.AutomaticallyOnStart);
        }

        private void OnEnable()
        {
            AutomaticallyInvoke(InvokeStrategy.AutomaticallyOnEnable);
        }

        private void AutomaticallyInvoke(InvokeStrategy strategy)
        {
            foreach (var audioEvent in m_audioEvents)
            {
                if (audioEvent.InvokeStrategy == strategy)
                {
                    PlayAudioEvent(audioEvent.Name);
                }
            }
        }

        public void PlayAudioEvent(string eventName)
        {
            foreach (var audioEvent in m_audioEvents)
            {
                if (audioEvent.Name.Equals(eventName))
                {
                    audioEvent.Play();
                }
            }
        }
    }
}