using System;
using System.Collections.Generic;
using Base.Logging;
using Base.Pattern;
using UniRx;
using UnityEngine;

namespace Base.Services
{
    /// <summary>
    /// This class is used to control the global sound of the project. For individual sound, please use another approach
    /// </summary>
    public class SoundService : MonoService, IService<AudioMappingConfiguration>
    {
        private AudioDataContainer m_audioDataContainer = null;

        private IDictionary<AudioGenre, AudioSource> m_audioSourceMap;


        private void OnSoundVolumeChanged(float value)
        {
            GetAudioSourceByGenre(AudioGenre.Sound).volume = value;
        }

        private void OnMusicVolumeChanged(float value)
        {
            GetAudioSourceByGenre(AudioGenre.Music).volume = value;
        }

        private void OnOneShotVolumeChanged(float value)
        {
            GetAudioSourceByGenre(AudioGenre.OneShot).volume = value;
        }

        public void UpdateData(AudioMappingConfiguration data)
        {
            m_audioDataContainer.UpdateAudioInfos(data);
        }
        public override void Init()
        {
            ServiceLocator.Set(this);
            m_audioDataContainer = new AudioDataContainer();
            m_audioSourceMap = new Dictionary<AudioGenre, AudioSource>();

            string[] source = Enum.GetNames(typeof(AudioGenre));
            for (int i = 0; i < source.Length; ++i)
            {
                string genreName = source[i];
                GameObject sourceObj = new GameObject($"AudioSource-{genreName}");
                AudioSource audioSource = sourceObj.AddComponent<AudioSource>();
                audioSource.transform.SetParent(CacheTransform);
                AudioGenre genre = Enum.Parse<AudioGenre>(genreName);
                if (!m_audioSourceMap.ContainsKey(genre))
                {
                    m_audioSourceMap.Add(genre, audioSource);
                }
                
                SetAudioVolume(genre, m_audioDataContainer.GetVolumeOf(genre));
            }
            
            m_audioDataContainer.GlobalMusicVolume.Subscribe(OnMusicVolumeChanged).AddTo(this);
            m_audioDataContainer.GlobalSoundVolume.Subscribe(OnSoundVolumeChanged).AddTo(this);
            m_audioDataContainer.GlobalOneShotVolume.Subscribe(OnOneShotVolumeChanged).AddTo(this);

            IsInitialize = true;
        }

        public bool IsAudioPlaying(AudioGenre genre) => m_audioSourceMap.TryGetValue(genre, out AudioSource source) && source.isPlaying;

        public void SetAudioVolume(AudioGenre genre, float value)
        {
            m_audioDataContainer.SetVolumeOf(genre, value);
        }

        public AudioSource GetAudioSourceByGenre(AudioGenre genre)
        {
            if (!m_audioSourceMap.TryGetValue(genre, out AudioSource source))
            {
                throw new ArgumentNullException($"AudioSource of type {genre.ToString()} is not initialized");
            }

            return source;
        }

        public void Play(AudioGenre genre, string audioKey)
        {
            AudioInfo audioInfo = m_audioDataContainer.GetAudioInfoOf(genre, audioKey);
            if (audioInfo == null)
            {
                return;
            }

            if (!m_audioSourceMap.TryGetValue(genre, out AudioSource source))
            {
                return;
            }

            if (genre is not AudioGenre.OneShot)
            {
                source.clip = audioInfo.AudioClip;
                source.loop = audioInfo.Loop;
                source.mute = audioInfo.Mute;
                source.Play();
            }
            else
            {
                source.PlayOneShot(audioInfo.AudioClip);
            }
        }

        public void Pause(AudioGenre genre, string audioKey)
        {
            AudioInfo audioInfo = m_audioDataContainer.GetAudioInfoOf(genre, audioKey);
            if (audioInfo == null)
            {
                return;
            }

            if (!m_audioSourceMap.TryGetValue(genre, out AudioSource source))
            {
                return;
            }
            
            source.Pause();
        }

        public void Resume(AudioGenre genre, string audioKey)
        {
            AudioInfo audioInfo = m_audioDataContainer.GetAudioInfoOf(genre, audioKey);
            if (audioInfo == null)
            {
                return;
            }

            if (!m_audioSourceMap.TryGetValue(genre, out AudioSource source))
            {
                return;
            }
            
            source.UnPause();
        }

        public void Stop(AudioGenre genre, string audioKey)
        {
            AudioInfo audioInfo = m_audioDataContainer.GetAudioInfoOf(genre, audioKey);
            if (audioInfo == null)
            {
                return;
            }

            if (!m_audioSourceMap.TryGetValue(genre, out AudioSource source))
            {
                return;
            }
            
            source.Stop();
        }

        public void Mute(AudioGenre genre, string audioKey, bool isMute)
        {
            AudioInfo audioInfo = m_audioDataContainer.GetAudioInfoOf(genre, audioKey);
            if (audioInfo == null)
            {
                return;
            }

            if (!m_audioSourceMap.TryGetValue(genre, out AudioSource source))
            {
                return;
            }

            source.mute = isMute;
        }
    }
}