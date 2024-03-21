#region Header
// Date: 20/03/2024
// Created by: Huynh Phong Tran
// File name: TimelineNode.cs
#endregion

using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

namespace Base.Helper
{
    [RequireComponent(typeof(PlayableDirector))]
    public class TimelineNode : BaseMono
    {
        [SerializeField] private PlayableDirector m_playableDirector;

        public async UniTask PlayAsync()
        {
            
        }
    }
}