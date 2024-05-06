#region Header
// Date: 20/03/2024
// Created by: Huynh Phong Tran
// File name: TimelineNode.cs
#endregion

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Base.Helper
{
    [RequireComponent(typeof(PlayableDirector))]
    [AddComponentMenu("Base Component/Timeline Node")]
    public class TimelineNode : BaseMono
    {
        [SerializeField] private PlayableDirector m_playableDirector;
        [SerializeField] private bool m_autoPlay = false;
        [SerializeField] private bool m_resetOnAwake = true;

        public PlayableDirector TimelineDirector
        {
            get
            {
                if (m_playableDirector == null)
                {
                    m_playableDirector = GetComponent<PlayableDirector>();
                }

                return m_playableDirector;
            }
        }
        public bool IsPlaying { get; private set; }

        private void Awake()
        {
            if (m_resetOnAwake)
            {
                TimelineDirector.initialTime = 0;
                TimelineDirector.time = 0;
                TimelineDirector.Evaluate();
            }
            
            TimelineDirector.stopped -= OnCompleted;
            TimelineDirector.stopped += OnCompleted;
        }

        private void OnEnable()
        {
            if (m_autoPlay) PlayIt();
        }

        private void OnCompleted(PlayableDirector director)
        {
            IsPlaying = false;
        }

        public void PlayIt(bool reset = true)
        {
            if (reset)
            {
                TimelineDirector.time = 0;
                TimelineDirector.Evaluate();
            }

            IsPlaying = true;
            TimelineDirector.Play();
        }

        public void StopIt(bool reset = false)
        {
            TimelineDirector.Stop();
            if (reset)
            {
                TimelineDirector.time = 0;
                TimelineDirector.Evaluate();
            }

            IsPlaying = false;
        }

        public void PauseIt()
        {
            TimelineDirector.Pause();
        }

        public void ResumeIt()
        {
            TimelineDirector.Resume();
        }
        /// <summary>
        /// Skip to an exact point of time
        /// </summary>
        /// <param name="time"></param>
        public void SkipTo(double time)
        {
            TimelineDirector.time = time;
            TimelineDirector.Evaluate();
        }

        public void SkipTo(int frame)
        {
            //TimelineDirector.time = frame / TimelineDirector.
        }
    }
}