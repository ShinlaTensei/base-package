#region Header
// Date: 31/12/2023
// Created by: Huynh Phong Tran
// File name: AudioEvent.cs
#endregion

using System;
using Base.Core;
using Base.Pattern;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Base.Core
{
    [Serializable]
    public class AudioEvent
    {
        [SerializeField, InfoBox("The name of audio event, this should be the same with audio key in configuration file and unique for the project")]
        protected string m_name;

        public string Name       => m_name;
        public string Type       { get; set; }
        public string SettingKey { get; set; } = SoundService.GLOBAL;

        [SerializeField, EnumToggleButtons] private InvokeStrategy m_invokeStrategy = InvokeStrategy.Manual;

        internal InvokeStrategy InvokeStrategy => m_invokeStrategy;

        public void Play()
        {
            ServiceLocator.Get<SoundService>().Play(Type, Name, SettingKey);
        }

        public void Pause()
        {
            ServiceLocator.Get<SoundService>().Pause(Type);
        }
        public void Resume()
        {
            ServiceLocator.Get<SoundService>().Resume(Type);
        }
        public void Stop()
        {
            ServiceLocator.Get<SoundService>().Stop(Type);
        }
    }

    internal enum InvokeStrategy
    {
        AutomaticallyOnStart, AutomaticallyOnEnable, Manual
    }
}