using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Base.Core;
using Base.Helper;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Base
{
    [Serializable]
    public class AudioAssetData : ReferenceData
    {
        /// <summary>
        /// Backing field of <see cref="ClipAssetKey"/>
        /// </summary>
        private string m_clipAssetKey;

        /// <summary>
        /// The addressable key for the audio
        /// </summary>
        public string ClipAssetKey
        {
            get => m_clipAssetKey;
            set => m_clipAssetKey = value;
        }

        /// <summary>
        /// Backing field of <see cref="CachedClips"/>
        /// </summary>
        private AudioClip m_cachedClip;

        /// <summary>
        /// A key-value mapping container that holds reference to the audio clip has been loaded
        /// </summary>
        public AudioClip CacheClip => m_cachedClip;

        private float m_volume;

        public float Volume
        {
            get => m_volume;
            set => m_volume = value;
        }

        private string m_assetGuid;

        public string AssetGuid => m_assetGuid;

        /// <summary>
        /// Empty constructor with default member value
        /// </summary>
        public AudioAssetData() {}
        
        /// <summary>
        /// Constructor setting the members to the provided values.
        /// </summary>
        /// <param name="index">The data's unique index.</param>
        /// <param name="objectName">The data's name.</param>
        /// <param name="guid">The referenced object's guid.</param>
        public AudioAssetData(int index, string objectName, string guid) : base(index, objectName, guid)
        {
            
        }

        public AudioAssetData(AudioAssetData original) : base(original)
        {
            CopyData(original);
        }

        public async UniTask<AudioClip> Evaluate(AddressableManager addressableService)
        {
            if (string.IsNullOrEmpty(ClipAssetKey))
            {
                throw new NullReferenceException($"NullReference: There is no asset with ID {ClipAssetKey} in audio data {ObjectName}");
            }
            if (!CacheClip)
            {
                m_cachedClip = await addressableService.LoadAsset<AudioClip>(ClipAssetKey, 3);
            }

            return CacheClip;
        }
    }
}

