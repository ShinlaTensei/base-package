#region Header
// Date: 26/12/2023
// Created by: Huynh Phong Tran
// File name: TimeData.cs
#endregion

using System;
using Base.Helper;
using Base.Pattern;

namespace Base
{
    public enum TimerType
    {
        CooldownInSecs,
        DailyUtc,
        Forever
    }
    public class TimerData
    {
        protected DateTime m_startTime;

        /// <summary>
        /// The raw duration which can be dependent of the Timer Type either seconds or days
        /// </summary>
        protected int m_duration;

        protected IDisposable m_runningInterval;
        protected Action<int> m_onTimerRunning;
        protected Action      m_onTimerFinished;
        
        public TimerType TimerType { get; protected set; }
        
        /// <summary>
        /// This always return the duration in seconds even if it is a daily timer
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public int DurationInSeconds
        {
            get
            {
                switch (TimerType)
                {
                    case TimerType.CooldownInSecs:
                        return m_duration;
                    case TimerType.DailyUtc:
                    case TimerType.Forever:
                        return (int)(GetFinishDateTime() - StartTime).TotalSeconds;
                    default:
                        throw new ArgumentOutOfRangeException($"No duration calculation present for Timer Type {TimerType}");
                }
            }
        }
        
        public DateTime StartTime { get => m_startTime; protected set => m_startTime = value; }
        
        public string TimerID { get; protected set; }

        public DateTime GetFinishDateTime() => TimerType switch
                                               {
                                                       TimerType.CooldownInSecs => StartTime.AddSeconds(m_duration),
                                                       TimerType.DailyUtc       => GetDailyFinishTimeUtc(),
                                                       TimerType.Forever        => DateTime.MaxValue,
                                                       _                        => throw new ArgumentOutOfRangeException()
                                               };
        private DateTime GetDailyFinishTimeUtc()
        {
            DateTime nextDay        = StartTime.AddDays(m_duration);
            DateTime nextDayClamped = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, 0, 0, 0, DateTimeKind.Utc);
            return nextDayClamped;
        }

        public bool IsFinished()
        {
            return ServiceLocator.Get<TimingService>().IsTimePassed(GetFinishDateTime());
        }

        public TimeSpan TimeLeftTillFinished()
        {
            return ServiceLocator.Get<TimingService>().TimeLeftUntil(GetFinishDateTime());
        }

        public TimeSpan TimePassedSinceStart()
        {
            return ServiceLocator.Get<TimingService>().TimePassedSince(StartTime);
        }
        
        public double GetTimeFinishedFactor()
        {
            return (TimePassedSinceStart().TotalSeconds / (GetFinishDateTime() - StartTime).TotalSeconds);
        }
        
        public int GetTimesFinished()
        {
            switch (TimerType)
            {
                case TimerType.CooldownInSecs:
                    return (int)(ServiceLocator.Get<TimingService>().TimePassedSince(StartTime).TotalSeconds / m_duration);
                case TimerType.DailyUtc:
                {
                    int daysPassed = 0;
                    if (IsFinished())
                    {
                        daysPassed++;
                    }

                    DateTime nextDay        = StartTime.AddDays(1);
                    DateTime nextDayClamped = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, 0, 0, 0, DateTimeKind.Utc);

                    return (int) (ServiceLocator.Get<TimingService>().TimePassedSince(nextDayClamped).TotalDays + daysPassed);
                }
                case TimerType.Forever:
                    return 0;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SetupTimer(Action<int> onTimerRunning, Action onTimerFinished)
        {
            m_onTimerFinished = onTimerFinished;
            m_onTimerRunning  = onTimerRunning;
            m_runningInterval = BaseInterval.RunInterval(1, OnTimerRunningAfterSetup);
        }

        public void StopRunningTimer()
        {
            if (m_runningInterval != null)
            {
                BaseInterval.StopInterval(m_runningInterval);
            }
        }

        protected void OnTimerRunningAfterSetup()
        {
            if (IsFinished())
            {
                m_onTimerFinished?.Invoke();
                StopRunningTimer();
                return;
            }
            
            m_onTimerRunning?.Invoke(DurationInSeconds);
        }
    }

    public class TimerDataMutable : TimerData
    {
        public void SetDuration(int duration)
        {
            m_duration = duration;
        }

        public void UpdateLastFinishedTimeStamp(DateTime startTimeStamp)
        {
            StartTime = startTimeStamp;
        }
        
        public TimerDataMutable(DateTime startTimeStamp, int duration, TimerType timerType = TimerType.CooldownInSecs)
        {
            TimerType = timerType;
            StartTime = startTimeStamp;
            TimerID   = Guid.NewGuid().ToString();
            m_duration = duration;
        }
        
        public TimerDataMutable(string uniqueId, DateTime startTimeStamp, int duration, TimerType timerType = TimerType.CooldownInSecs)
        {
            TimerType  = timerType;
            StartTime  = startTimeStamp;
            TimerID    = uniqueId;
            m_duration = duration;
        }
        
        public TimerDataMutable(string uniqueId, int duration, TimerType timerType = TimerType.CooldownInSecs)
        {
            TimerType  = timerType;
            StartTime  = ServiceLocator.Get<TimingService>().GetCurrentTime();
            TimerID    = uniqueId;
            m_duration = duration;
        }
    }
}