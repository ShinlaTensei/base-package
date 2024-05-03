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
        private IDictionary<string, AudioSource> MapAudioSource { get; set; }

        private CancellationTokenSource CancellationToken { get; set; }

        public void Init(AudioDataContainer dataContainer)
        {
            Init();
            AudioDataContainer = dataContainer;
            AddressableManager = ServiceLocator.Get<AddressableManager>();

            MapAudioSource = new Dictionary<string, AudioSource>();

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

            AudioClip clip = await audioAssetData.Evaluate(AddressableManager, audioKey)
                .AttachExternalCancellation(CancellationToken.Token);

            AudioSettingData settingData = audioAssetData.CreateOrGetSettingData(audioKey);

            AudioSource audioSource = GetOrCreateAudioSource(audioKey);

            audioSource.volume = settingData.Volume;
            audioSource.clip = clip;

            if (settingData.PlayOneShot)
            {
                audioSource.PlayOneShot(clip);
                return;
            }

            if (delay > 0f)
            {
                audioSource.PlayDelayed(delay);
            }
            else
            {
                audioSource.Play();
            }
        }

        private async UniTask PauseAsync(string audioKey, float delayPause)
        {
            AudioSource audioSource = GetOrCreateAudioSource(audioKey);
            if (!audioSource.isPlaying) return;

            if (!AudioDataContainer.TryGetData(audioKey, out AudioAssetData audioAssetData))
            {
            }

            AudioClip clip = await audioAssetData.Evaluate(AddressableManager, audioKey)
                .AttachExternalCancellation(CancellationToken.Token);

            if (audioSource.clip == null || audioSource.clip != clip) return;

            await UniTask.Delay(TimeSpan.FromSeconds(delayPause));

            audioSource.Pause();
        }

        private async UniTask ResumeAsync(string audioKey, float delay)
        {
            AudioSource audioSource = GetOrCreateAudioSource(audioKey);
            if (!audioSource.isPlaying) return;

            if (!AudioDataContainer.TryGetData(audioKey, out AudioAssetData audioAssetData))
            {
            }

            AudioClip clip = await audioAssetData.Evaluate(AddressableManager, audioKey)
                .AttachExternalCancellation(CancellationToken.Token);

            if (audioSource.clip == null || audioSource.clip != clip) return;

            await UniTask.Delay(TimeSpan.FromSeconds(delay));

            audioSource.Play();
        }

        private async UniTask StopAsync(string audioKey, float delay)
        {
            AudioSource audioSource = GetOrCreateAudioSource(audioKey);
            if (!audioSource.isPlaying) return;

            if (!AudioDataContainer.TryGetData(audioKey, out AudioAssetData audioAssetData))
            {
            }

            AudioClip clip = await audioAssetData.Evaluate(AddressableManager, audioKey)
                .AttachExternalCancellation(CancellationToken.Token);

            if (audioSource.clip == null || audioSource.clip != clip) return;

            await UniTask.Delay(TimeSpan.FromSeconds(delay));

            audioSource.Stop();
        }

        private AudioSource GetOrCreateAudioSource(string key)
        {
            if (!MapAudioSource.TryGetValue(key, out AudioSource source))
            {
                GameObject go = new GameObject(key);
                source = go.AddComponent<AudioSource>();
                source.playOnAwake = false;
                go.transform.SetParent(CacheTransform);
                MapAudioSource[key] = source;
            }

            return source;
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