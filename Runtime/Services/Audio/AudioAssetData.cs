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
        [NonSerialized] private WeakReference<AudioClip> m_cachedClip;

        /// <summary>
        /// A key-value mapping container that holds reference to the audio clip has been loaded
        /// </summary>
        public WeakReference<AudioClip> CacheClip
        {
            get
            {
                if (m_cachedClip == null)
                {
                    m_cachedClip = new WeakReference<AudioClip>(null);
                }

                return m_cachedClip;
            }
        }

        private float m_volume;

        public float Volume
        {
            get => m_volume;
            set => m_volume = value;
        }
        
        /// <summary>
        /// Backing field of <see cref="AssetGuid"/>
        /// </summary>
        private string m_assetGuid;

        /// <summary>
        /// The GUID of the audio clip that this asset holds the addressable key in <see cref="ClipAssetKey"/>
        /// </summary>
        public string AssetGuid
        {
            get => m_assetGuid;
            set => m_assetGuid = value;
        }
        /// <summary>
        /// Backing field of <see cref="SourceObject"/>
        /// </summary>
        private AudioClip m_sourceObject;
        /// <summary>
        /// A prefab contain Audio Source component to play this asset
        /// </summary>
        public AudioClip SourceObject
        {
            get => m_sourceObject;
            set => m_sourceObject = value;
        }

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
            ClipAssetKey = original.ClipAssetKey;
            AssetGuid = original.AssetGuid;
            Volume = original.Volume;
        }

        public async UniTask<AudioClip> Evaluate(AddressableManager addressableService)
        {
            if (string.IsNullOrEmpty(ClipAssetKey))
            {
                throw new NullReferenceException($"NullReference: There is no asset with ID {ClipAssetKey} in audio data {ObjectName}");
            }
            if (!CacheClip.TryGetTarget(out AudioClip clip))
            {
                clip = await addressableService.LoadAsset<AudioClip>(ClipAssetKey, 3);
                m_cachedClip.SetTarget(clip);
            }

            return clip;
        }
    }
}

