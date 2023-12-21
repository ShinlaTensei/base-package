#region Header
// Date: 31/12/2023
// Created by: Huynh Phong Tran
// File name: AudioEvent.cs
#endregion

using System;
using Base.Pattern;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Base.Services
{
    [Serializable]
    public abstract class AudioEvent
    {
        [SerializeField, InfoBox("The name of audio event, this should be the same with audio key in configuration file and unique for the project")]
        protected string m_name;

        public string Name => m_name;

        [SerializeField, EnumToggleButtons] private InvokeStrategy m_invokeStrategy = InvokeStrategy.Manual;

        internal InvokeStrategy InvokeStrategy => m_invokeStrategy;

        [SerializeField, Range(0, 1)] protected float m_volume;

        [SerializeField] protected bool m_overrideSetting = false;

        public abstract void Play();
        public abstract void Pause();
        public abstract void Resume();
        public abstract void Stop();
    }

    public class OneShotAudioEvent : AudioEvent
    {
        public override void Play()
        {
            if (m_overrideSetting)
            {
                ServiceLocator.Get<SoundService>().SetAudioVolume(AudioGenre.OneShot, m_volume);
            }
            
            ServiceLocator.Get<SoundService>().Play(AudioGenre.OneShot, Name);
        }
        public override void Pause()
        {
            
        }
        public override void Resume()
        {
            
        }
        public override void Stop()
        {
            
        }
    }

    public class MusicAudioEvent : AudioEvent
    {
        public override void Play()
        {
            if (m_overrideSetting)
            {
                ServiceLocator.Get<SoundService>().SetAudioVolume(AudioGenre.Music, m_volume);
            }
            
            ServiceLocator.Get<SoundService>().Play(AudioGenre.Music, Name);
        }
        public override void Pause()
        {
            ServiceLocator.Get<SoundService>().Pause(AudioGenre.Music, Name);
        }
        public override void Resume()
        {
            ServiceLocator.Get<SoundService>().Resume(AudioGenre.Music, Name);
        }
        public override void Stop()
        {
            ServiceLocator.Get<SoundService>().Stop(AudioGenre.Music, Name);
        }
    }

    public class SoundAudioEvent : AudioEvent
    {
        public override void Play()
        {
            if (m_overrideSetting)
            {
                ServiceLocator.Get<SoundService>().SetAudioVolume(AudioGenre.Sound, m_volume);
            }
            
            ServiceLocator.Get<SoundService>().Play(AudioGenre.Sound, Name);
        }
        public override void Pause()
        {
            ServiceLocator.Get<SoundService>().Pause(AudioGenre.Sound, Name);
        }
        public override void Resume()
        {
            ServiceLocator.Get<SoundService>().Resume(AudioGenre.Sound, Name);
        }
        public override void Stop()
        {
            ServiceLocator.Get<SoundService>().Stop(AudioGenre.Sound, Name);
        }
    }

    internal enum InvokeStrategy
    {
        AutomaticallyOnStart, AutomaticallyOnEnable, Manual
    }
}