using System.Collections.Generic;
using Base.Core;
using Base.Helper;
using UniRx;
using UnityEngine;

namespace Base.Services
{
    public class AudioDataContainer : DataContainer<AudioAssetData>
    {
        public override List<AudioAssetData> DataCollection { get; protected set; }
        public override List<AudioAssetData> WorkingCopy { get; protected set; }
    }
}