#region Header
// Date: 28/12/2023
// Created by: Huynh Phong Tran
// File name: ITimingService.cs
#endregion

using System;
using Base.Helper;
using Base.Pattern;
using UnityEngine;

namespace Base
{
    public class TimingService :Core.Service
    {
        private DateTime m_lastFetchTime = DateTime.MinValue;
        private float    m_realTimeSinceStartUpBase;
        private int      m_utcOffSet;

        private DateTime TimeNow      => m_lastFetchTime.AddSeconds(Time.realtimeSinceStartup - m_realTimeSinceStartUpBase);
        private DateTime TimeNowLocal => m_lastFetchTime.AddSeconds(Time.realtimeSinceStartup - m_realTimeSinceStartUpBase).AddHours(m_utcOffSet);
        public void SetTimeInitialize(DateTime startTime)
        {
            m_lastFetchTime            = startTime.AddSeconds(1);
            m_realTimeSinceStartUpBase = Time.realtimeSinceStartup;
        }

        public void SetUtcOffSet(int offset)
        {
            m_utcOffSet = offset;
        }

        public int GetUtcOffSet() => m_utcOffSet;

        public TimeSpan TimeLeftUntil(DateTime endTime)
        {
            return endTime - GetCurrentTime();
        }

        public DateTime GetCurrentTime()
        {
            return TimeNow;
        }

        public bool IsTimePassed(DateTime endTime)
        {
            return TimeLeftUntil(endTime).TotalMilliseconds <= 0;
        }

        public TimeSpan TimePassedSince(DateTime targetTime)
        {
            return GetCurrentTime() - targetTime;
        }

        public DateTime FirstDateOfWeek(DateTime referenceDate, DayOfWeek weekStartDay = DayOfWeek.Monday)
        {
            while (referenceDate.DayOfWeek != weekStartDay)
            {
                referenceDate = referenceDate.AddDays(-1);
            }

            return referenceDate;
        }

        public DateTime EndOfDay(DateTime referenceDate)
        {
            return referenceDate.Date.AddDays(1).AddTicks(-1);
        }

        public DateTime EndOfWeek(DateTime referenceDate, DayOfWeek weekStartDay = DayOfWeek.Monday)
        {
            return EndOfDay(FirstDateOfWeek(referenceDate, weekStartDay)).AddDays(7);
        }
        
        public DateTime GetFirstDayOfNextMonth(DateTime referenceDate)
        {
            return new DateTime((referenceDate.Month == 12 ? referenceDate.Year + 1 : referenceDate.Year), (referenceDate.Month == 12 ? 1 : referenceDate.Month + 1), 1, 12, 0, 0);
        }
        
        public DateTime GetFirstDayOfPrevMonth(DateTime referenceDate)
        {
            return new DateTime((referenceDate.Month == 1 ? referenceDate.Year - 1 : referenceDate.Year), (referenceDate.Month == 1 ? 12 : referenceDate.Month - 1), 1, 12, 0, 0);
        }
        
        public DateTime StartOfMonth(DateTime referenceDate)
        {
            return new DateTime(referenceDate.Year, referenceDate.Month, 1);
        }
        
        public DateTime EndOfMonth(DateTime referenceDate)
        {
            return StartOfMonth(referenceDate).AddMonths(1).AddSeconds(-1);
        }
        public bool IsAfterDay(DateTime referenceDate, DateTime targetDate)
        {
            return DateTime.Compare(referenceDate, targetDate) > 0 && !IsSameDay(referenceDate, targetDate);
        }
       
        public bool IsBeforeDay(DateTime referenceDate, DateTime targetDate)
        {
            return DateTime.Compare(referenceDate, targetDate) < 0 && !IsSameDay(referenceDate, targetDate);
        }
        
        public bool IsDayAfterToday(DateTime targetDate)
        {
            return IsAfterDay(targetDate, TimeNow);
        }

        public bool IsDayBeforeToday(DateTime targetDate)
        {
            return IsBeforeDay(targetDate, TimeNow);
        }

        public bool IsSameDay(DateTime referenceDate, DateTime targetDate)
        {
            return referenceDate.Day == targetDate.Day && referenceDate.Month == targetDate.Month && referenceDate.Year == targetDate.Year;
        }
        
        public bool IsToday(DateTime date)
        {
            return IsSameDay(date, TimeNow);
        }
        
        public DateTime StartOfDay(DateTime referenceDate)
        {
            return referenceDate.Date;
        }
        
        public DateTime StartOfNextClosestDay(DateTime referenceDate, DayOfWeek nextDay)
        {
            int dayCount = BaseMathf.Mod((int)nextDay - (int)referenceDate.DayOfWeek + 7, 7);
            return referenceDate.Date.AddDays(dayCount);
        }
        
        public DateTime ClampToHighNoon(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 12, 0, 0);
        }

        public DateTime ClampToHighNoonFirstOfMonth(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1, 12, 0, 0);
        }

#region Time Repository
        
#endregion
    }
}