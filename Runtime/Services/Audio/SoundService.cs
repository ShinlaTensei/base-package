using System;
using System.Collections.Generic;
using Base.Core;
using Base.Helper;
using Base.Logging;
using Base.Pattern;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Base.Services
{
    /// <summary>
    /// This class is used to control the global sound of the project. For individual sound, please use another approach
    /// </summary>
    public class SoundService : MonoService
    {
        public const string GLOBAL = "Global";

        private AudioDataContainer m_audioDataContainer = null;

        private IDictionary<string, AudioSource> m_audioSourceMap;

        private AddressableManager m_addressableManager;
        
        public override void Init()
        {
            ServiceLocator.Set(this);
            m_audioDataContainer = new AudioDataContainer();
            m_audioSourceMap     = new Dictionary<string, AudioSource>();
            m_addressableManager = ServiceLocator.Get<AddressableManager>();

            IsInitialize = true;
        }

        public bool IsAudioPlaying(string audioType) => m_audioSourceMap.TryGetValue(audioType, out AudioSource source) && source.isPlaying;

        public AudioSource GetAudioSourceByType(string audioType)
        {
            if (!m_audioSourceMap.TryGetValue(audioType, out AudioSource source))
            {
                PDebug.Warn($"AudioSource of type {audioType} is not initialized");
                PDebug.InfoFormat("Start Create audio source for type {0}", audioType);
                GameObject sourceObj = new GameObject($"AudioSource-{audioType}");
                source = sourceObj.AddComponent<AudioSource>();
                sourceObj.transform.SetParent(CacheTransform);
            }

            return source;
        }

        private async UniTask PlayAsync(string audioType, string audioKey , string settingKey = GLOBAL, bool isOneTime = false)
        {
            if (!m_audioDataContainer.TryGetData(audioType, out AudioAssetData audioInfo))
            {
                return;
            }

            AudioClip clip = await audioInfo.Evaluate(m_addressableManager, audioKey)
                                            .AttachExternalCancellation(TaskRunner.GetCancellationTokenForType(nameof(SoundService)));
            AudioSource audioSource = GetAudioSourceByType(audioType);
            
            AudioSetting setting = m_audioDataContainer.GetSetting<AudioSetting>(settingKey);
            audioSource.volume = setting.Volume;
            audioSource.loop   = setting.Loop;

            if (clip != null)
            {
                audioSource.clip = clip;
                if (isOneTime)
                {
                    audioSource.PlayOneShot(clip);
                }
                else
                {
                    audioSource.Play();
                }
            }
        }

        public void Play(string audioType, string audioKey, string settingKey = GLOBAL)
        {
            TaskRunner.Start(PlayAsync(audioType, audioKey, settingKey).AsTask());
        }

        public void PlayOneShot(string audioType, string audioKey, string settingKey = GLOBAL)
        {
            TaskRunner.Start(PlayAsync(audioType, audioKey, settingKey, true).AsTask());
        }

        public void Pause(string audioType)
        {
            if (!m_audioDataContainer.TryGetData(audioType, out AudioAssetData audioInfo))
            {
                return;
            }
            
            AudioSource audioSource = GetAudioSourceByType(audioType);
            audioSource.Pause();
        }

        public void Resume(string audioType)
        {
            if (!m_audioDataContainer.TryGetData(audioType, out AudioAssetData audioInfo))
            {
                return;
            }

            AudioSource audioSource = GetAudioSourceByType(audioType);
            audioSource.UnPause();
        }

        public void Stop(string audioType)
        {
            if (!m_audioDataContainer.TryGetData(audioType, out AudioAssetData audioInfo))
            {
                return;
            }

            AudioSource audioSource = GetAudioSourceByType(audioType);
            audioSource.Stop();
        }

        public void Mute(string audioType)
        {
            if (!m_audioDataContainer.TryGetData(audioType, out AudioAssetData audioInfo))
            {
                return;
            }

            AudioSource audioSource = GetAudioSourceByType(audioType);
            audioSource.mute = true;
        }
        
        public void UnMute(string audioType)
        {
            if (!m_audioDataContainer.TryGetData(audioType, out AudioAssetData audioInfo))
            {
                return;
            }

            AudioSource audioSource = GetAudioSourceByType(audioType);
            audioSource.mute = false;
        }
    }
}