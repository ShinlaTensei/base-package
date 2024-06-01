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
using UnityEngine;

namespace Base
{
    public class AudioService : Service<AudioDataContainer>
    {
        private AudioDataContainer AudioDataContainer { get; set; }
        private AddressableManager AddressableManager { get; set; }

        private CancellationTokenSource CancellationToken { get; set; }
        
        private AudioSource AudioSource { get; set; }

        public override void Init()
        {
            AddressableManager = ServiceLocator.Get<AddressableManager>();

            CancellationToken = new CancellationTokenSource();
            AudioSource = BaseContextRegistry.TryGetOrCreateContext(0).Get<AudioSource>();
        }

        public override void UpdateData(AudioDataContainer data)
        {
            AudioDataContainer = data;
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

        private async UniTask PlayAsync(AudioAssetData audioAssetData)
        {
            AudioClip clip = await audioAssetData.Evaluate(AddressableManager);

            AudioSource.clip = clip;
            AudioSource.volume = audioAssetData.Volume;
            AudioSource.Play();
        }
        
        private async UniTask PlayOneshotAsync(AudioAssetData audioAssetData)
        {
            AudioClip clip = await audioAssetData.Evaluate(AddressableManager);
            
            AudioSource.volume = audioAssetData.Volume;
            AudioSource.PlayOneShot(clip);
        }

        public override void Dispose()
        {
            if (CancellationToken != null && !CancellationToken.IsCancellationRequested)
            {
                CancellationToken.Cancel();
                CancellationToken.Dispose();
            }
        }
    }
}