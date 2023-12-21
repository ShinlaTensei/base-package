using System.Collections.Generic;
using Base.Helper;
using UniRx;
using UnityEngine;

namespace Base.Services
{
    public enum AudioGenre
    {
        Music, OneShot, Sound
    }
    public class AudioDataContainer
    {
        private IDictionary<string, AudioInfo> m_musicAudioMap;
        private IDictionary<string, AudioInfo> m_oneShotAudioMap;
        private IDictionary<string, AudioInfo> m_soundAudioMap;
        /// <summary>
        /// Global Volume
        /// </summary>
        public FloatReactiveProperty GlobalVolume { get; private set; }
        /// <summary>
        /// Global volume for music
        /// </summary>
        public FloatReactiveProperty GlobalMusicVolume { get; private set; }
        /// <summary>
        /// Global volume for one shot audio
        /// </summary>
        public FloatReactiveProperty GlobalOneShotVolume { get; private set; }
        /// <summary>
        /// Global volume for sound
        /// </summary>
        public FloatReactiveProperty GlobalSoundVolume { get; private set; }


        private IDictionary<string, AudioInfo> ConvertFromConfig(List<AudioInfoConfig> configs)
        {
            IDictionary<string, AudioInfo> result = new Dictionary<string, AudioInfo>();

            for (int i = 0; i < configs.Count; ++i)
            {
                result[configs[i].AudioKey] = new AudioInfo(configs[i]);
            }

            return result;
        }

        public AudioDataContainer()
        {
            m_musicAudioMap   = new Dictionary<string, AudioInfo>();
            m_oneShotAudioMap = new Dictionary<string, AudioInfo>();
            m_soundAudioMap   = new Dictionary<string, AudioInfo>();

            GlobalVolume = GlobalMusicVolume = GlobalOneShotVolume = GlobalSoundVolume = new FloatReactiveProperty(1);
        }

        public void UpdateAudioInfos(AudioMappingConfiguration configuration)
        {
            m_musicAudioMap   = new Dictionary<string, AudioInfo>(ConvertFromConfig(configuration.GetAudioConfig(AudioGenre.Music)));
            m_soundAudioMap   = new Dictionary<string, AudioInfo>(ConvertFromConfig(configuration.GetAudioConfig(AudioGenre.Sound)));
            m_oneShotAudioMap = new Dictionary<string, AudioInfo>(ConvertFromConfig(configuration.GetAudioConfig(AudioGenre.OneShot)));
        }
        

        public float GetVolumeOf(AudioGenre genre) => genre switch
        {
            AudioGenre.Music => GlobalMusicVolume.Value,
            AudioGenre.Sound => GlobalSoundVolume.Value,
            AudioGenre.OneShot => GlobalOneShotVolume.Value,
            _ => GlobalVolume.Value
        };

        public void SetVolumeOf(AudioGenre genre, float value)
        {
            switch (genre)
            {
                case AudioGenre.Music: GlobalMusicVolume.Value = value;
                    break;
                case AudioGenre.Sound: GlobalSoundVolume.Value = value;
                    break;
                case AudioGenre.OneShot: GlobalOneShotVolume.Value = value;
                    break;
            }
        }

        public AudioInfo GetAudioInfoOf(AudioGenre genre, string audioKey)
        {
            AudioInfo audioInfo;
            switch (genre)
            {
                case AudioGenre.Music:
                    m_musicAudioMap.TryGetValue(audioKey, out audioInfo);
                    break;
                case AudioGenre.Sound:
                    m_soundAudioMap.TryGetValue(audioKey, out audioInfo);
                    break;
                case AudioGenre.OneShot:
                    m_oneShotAudioMap.TryGetValue(audioKey, out audioInfo);
                    break;
                default:
                    audioInfo = default;
                    break;
            }

            return audioInfo;
        }
    }
}