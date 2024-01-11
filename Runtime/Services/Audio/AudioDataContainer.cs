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

        private IDictionary<string, AudioAssetData> m_audioAsset;

        private IDictionary<string, AudioAssetData> AudioAssets => m_audioAsset ??= new Dictionary<string, AudioAssetData>();


        private IDictionary<string, AudioInfo> ConvertFromConfig(List<AudioInfoConfig> configs)
        {
            IDictionary<string, AudioInfo> result = new Dictionary<string, AudioInfo>();

            for (int i = 0; i < configs.Count; ++i)
            {
                result[configs[i].AudioKey] = new AudioInfo(configs[i]);
            }

            return result;
        }

        public AudioDataContainer() { }

        public void UpdateAudioInfos(AudioMappingConfiguration configuration)
        {
            
        }

        public AudioAssetData GetAudioInfoOf(string audioType)
        {
            if (!m_audioAsset.TryGetValue(audioType, out var audioAssetData))
            {
                return null;
            }

            return audioAssetData;
        }
    }
}