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
    public class AudioService : MonoService
    {
        private AudioDataContainer AudioDataContainer { get; set; }
        private AddressableManager AddressableManager { get; set; }
        
        private AudioSource AudioSource { get; set; }
        
        private CancellationTokenSource CancellationToken { get; set; } 

        public void Init(AudioDataContainer dataContainer)
        {
            Init();
            AudioDataContainer = dataContainer;
            AddressableManager = ServiceLocator.Get<AddressableManager>();
            
            AudioSource        = new GameObject("AudioSource").AddComponent<AudioSource>();
            AudioSource.transform.SetParent(CacheTransform);
            AudioSource.playOnAwake = false;

            CancellationToken = new CancellationTokenSource();
        }

        public override void Dispose()
        {
            if (CancellationToken != null && !CancellationToken.IsCancellationRequested)
            {
                CancellationToken.Cancel();
                CancellationToken.Dispose();
            }
        }

        private async UniTask PlayAsync(string audioKey, float delay = 0f)
        {
            if (AudioDataContainer == null) return;

            if (!AudioDataContainer.TryGetData(audioKey, out AudioAssetData audioAssetData))
            {
                
            }

            AudioClip clip = await audioAssetData.Evaluate(AddressableManager, audioKey).AttachExternalCancellation(CancellationToken.Token);

            AudioSettingData settingData = audioAssetData.CreateOrGetSettingData(audioKey);

            AudioSource.volume = settingData.Volume;
            AudioSource.clip   = clip;

            if (settingData.PlayOneShot)
            {
                AudioSource.PlayOneShot(clip);
                return;
            }

            if (delay > 0f)
            {
                AudioSource.PlayDelayed(delay);
            }
            else
            {
                AudioSource.Play();
            }
        }

        private async UniTask PauseAsync(string audioKey, float delayPause)
        {
            if (!AudioSource.isPlaying) return;
            
            if (!AudioDataContainer.TryGetData(audioKey, out AudioAssetData audioAssetData))
            {
                
            }

            AudioClip clip = await audioAssetData.Evaluate(AddressableManager, audioKey).AttachExternalCancellation(CancellationToken.Token);

            if (AudioSource.clip == null || AudioSource.clip != clip) return;

            await UniTask.Delay(TimeSpan.FromSeconds(delayPause));
            
            AudioSource.Pause();
        }
        
        private async UniTask ResumeAsync(string audioKey, float delay)
        {
            if (!AudioSource.isPlaying) return;
            
            if (!AudioDataContainer.TryGetData(audioKey, out AudioAssetData audioAssetData))
            {
                
            }

            AudioClip clip = await audioAssetData.Evaluate(AddressableManager, audioKey).AttachExternalCancellation(CancellationToken.Token);

            if (AudioSource.clip == null || AudioSource.clip != clip) return;

            await UniTask.Delay(TimeSpan.FromSeconds(delay));
            
            AudioSource.Play();
        }
        
        private async UniTask StopAsync(string audioKey, float delay)
        {
            if (!AudioSource.isPlaying) return;
            
            if (!AudioDataContainer.TryGetData(audioKey, out AudioAssetData audioAssetData))
            {
                
            }

            AudioClip clip = await audioAssetData.Evaluate(AddressableManager, audioKey).AttachExternalCancellation(CancellationToken.Token);

            if (AudioSource.clip == null || AudioSource.clip != clip) return;

            await UniTask.Delay(TimeSpan.FromSeconds(delay));
            
            AudioSource.Stop();
        }

        public void Play(string audioKey, float delay = 0f)
        {
            TaskRunner.Start(PlayAsync(audioKey, delay).AsTask());
        }

        public void Pause(string audioKey, float delayPause = 0f)
        {
            TaskRunner.Start(PauseAsync(audioKey, delayPause).AsTask());
        }

        public void Resume(string audioKey, float delay = 0f)
        {
            TaskRunner.Start(ResumeAsync(audioKey, delay).AsTask());
        }

        public void Stop(string audioKey, float delay = 0f)
        {
            TaskRunner.Start(StopAsync(audioKey, delay).AsTask());
        }
    }
}