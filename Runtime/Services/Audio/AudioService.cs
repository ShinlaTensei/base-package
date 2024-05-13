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
    public class AudioService : Service
    {
        private AudioDataContainer AudioDataContainer { get; set; }
        private AddressableManager AddressableManager { get; set; }

        private CancellationTokenSource CancellationToken { get; set; }

        public void Init(AudioDataContainer dataContainer)
        {
            Init();
            AudioDataContainer = dataContainer;
            AddressableManager = ServiceLocator.Get<AddressableManager>();

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
    }
}