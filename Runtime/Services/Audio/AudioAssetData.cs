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
        /// Backing field of <see cref="ClipAssetKeys"/>
        /// </summary>
        private List<string> m_clipAssetKeys;
        
        /// <summary>
        /// A list contains all the audio clip addressable keys
        /// </summary>
        public List<string> ClipAssetKeys
        {
            get => LazyInitializer.EnsureInitialized(ref m_clipAssetKeys);
            set => m_clipAssetKeys = value;
        }
        
        /// <summary>
        /// Backing field of <see cref="CachedClips"/>
        /// </summary>
        private Dictionary<string, AudioClip> m_cachedClips;

        private bool m_isOneShot = false;

        /// <summary>
        /// A key-value mapping container that holds reference to the audio clip has been loaded
        /// </summary>
        public Dictionary<string, AudioClip> CachedClips => LazyInitializer.EnsureInitialized(ref m_cachedClips);

        public bool IsOneShot
        {
            set => m_isOneShot = value;
            get => m_isOneShot;
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
            CopyData(original);
        }

        public async UniTask<AudioClip> Evaluate(AddressableManager addressableService, string clipId)
        {
            if (!ClipAssetKeys.Contains(clipId))
            {
                throw new NullReferenceException($"NullReference: There is no asset with ID {clipId} in data of type {Type}");
            }
            if (!CachedClips.TryGetValue(clipId, out AudioClip clip))
            {
                clip = await addressableService.LoadAsset<AudioClip>(clipId, 3);

                if (clip != null)
                {
                    CachedClips[clipId] = clip;
                }
            }

            return clip;
        }
        
        public override void CopyData(StringData data)
        {
            base.CopyData(data);

            if (data is AudioAssetData audioAssetData)
            {
                m_clipAssetKeys = new List<string>();
                m_clipAssetKeys.AddRange(audioAssetData.ClipAssetKeys);
            }
        }
    }
}

