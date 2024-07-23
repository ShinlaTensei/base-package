#region Header

// Date: 18/02/2024
// Created by: Huynh Phong Tran
// File name: AudioService.cs

#endregion

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Base.Core;
using Base.Helper;
using Base.Pattern;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Base
{
    public class AudioService : Service<AudioDataContainer>
    {
        private AudioDataContainer AudioDataContainer { get; set; }
        private AddressableManager AddressableManager { get; set; }

        private IDictionary<string, AudioSource> AudioSourceMap { get; set; } = new Dictionary<string, AudioSource>();

        private CancellationTokenSource CancellationToken { get; set; }

        public override void Init()
        {
            AddressableManager = ServiceLocator.Get<AddressableManager>();

            CancellationToken = new CancellationTokenSource();
        }

        public override void UpdateData(AudioDataContainer data)
        {
            AudioDataContainer = data;
            
            PoolSystem.CreatePool(AudioDataContainer.AudioSourcePrefab);
        }

        public void Play(AudioEventBase eventBase)
        {
            if (!AudioDataContainer.TryGetData(eventBase.AudioKey, out AudioAssetData audioAssetData))
            {
                return;
            }

            if (eventBase is OneShotAudioEvent)
            {
                TaskRunner.Start(PlayOneshotAsync(audioAssetData).AsTask());
                return;
            }
            TaskRunner.Start(PlayAsync(audioAssetData).AsTask());
        }

        public void Pause(AudioAssetData audioAssetData)
        {
            if (AudioSourceMap.TryGetValue(audioAssetData.ObjectName, out AudioSource source))
            {
                source.Pause();
            }
        }

        public void Resume(AudioAssetData audioAssetData)
        {
            if (AudioSourceMap.TryGetValue(audioAssetData.ObjectName, out AudioSource source))
            {
                source.Play();
            }
        }

        public void Stop(AudioAssetData audioAssetData)
        {
            if (AudioSourceMap.TryGetValue(audioAssetData.ObjectName, out AudioSource source))
            {
                source.Stop();
            }
        }

        private async UniTask PlayAsync(AudioAssetData audioAssetData)
        {
            AudioClip clip = await audioAssetData.Evaluate(AddressableManager).AttachExternalCancellation(CancellationToken.Token);
            
            AudioSource source = PoolSystem.Rent<AudioSource>(Vector3.zero, Quaternion.identity, PoolSystem.Instance.CacheTransform);
            AudioSourceMap[audioAssetData.ObjectName] = source;
            source.clip = clip;
            source.volume = audioAssetData.Volume;
            source.Play();
            BaseInterval.RunAfterTime(clip.length, () => PoolSystem.Return(source)).AddTo(CancellationToken.Token);
        }
        
        private async UniTask PlayOneshotAsync(AudioAssetData audioAssetData)
        {
            AudioClip clip = await audioAssetData.Evaluate(AddressableManager).AttachExternalCancellation(CancellationToken.Token);
            
            AudioSource source = PoolSystem.Rent<AudioSource>(Vector3.zero, Quaternion.identity, PoolSystem.Instance.CacheTransform);
            AudioSourceMap[audioAssetData.ObjectName] = source;
            
            source.volume = audioAssetData.Volume;
            source.PlayOneShot(clip);
            BaseInterval.RunAfterTime(clip.length, () => PoolSystem.Return(source)).AddTo(CancellationToken.Token);
        }

        public override void Dispose()
        {
            if (CancellationToken != null && !CancellationToken.IsCancellationRequested)
            {
                CancellationToken.Cancel();
                CancellationToken.Dispose();
            }
            
            AddressableManager = null;
            AudioDataContainer = null;
            AudioSourceMap.Clear();
        }
    }
}