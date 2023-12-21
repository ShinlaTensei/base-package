#region Header
// Date: 21/12/2023
// Created by: Huynh Phong Tran
// File name: IAudioInfo.cs
#endregion

using System;
using UnityEngine;

namespace Base.Services
{
    public record AudioInfo
    {
        public string AudioID { get; private set; }
        public string AudioKey { get; private set; }
        public bool Is3DAudio { get; set; }
        public bool Loop { get; private set; } = false;
        public bool Mute { get; private set; } = false;

        public AudioClip AudioClip { get; private set; }

        public AudioInfo()
        {
            AudioID = Guid.NewGuid().ToString();
        }

        public AudioInfo(string audioKey, AudioClip audioClip, bool is3DAudio)
        {
            AudioID = Guid.NewGuid().ToString();
            AudioKey = audioKey;
            Is3DAudio = is3DAudio;
            AudioClip = audioClip;
        }

        public AudioInfo(string audioID, string audioKey, AudioClip audioClip, bool is3DAudio)
        : this(audioKey, audioClip, is3DAudio)
        {
            AudioID = audioID;
        }

        public AudioInfo(AudioInfoConfig config)
        {
            AudioID   = config.AudioId;
            AudioKey  = config.AudioKey;
            AudioClip = config.AudioClip;
            Loop      = config.Loop;
            Mute      = config.Mute;
            Is3DAudio = config.Is3dAudio;
        }
    }
}