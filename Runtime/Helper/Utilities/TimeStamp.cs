using System;
using System.Globalization;
using UnityEngine;

namespace Base.Helper
{
    public static class TimeStamp
    {
        public static DateTime EpochUTC = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static DateTime EpochLocal = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
        
        /// <summary>
        /// Get time UTC in seconds.
        /// </summary>
        /// <returns> seconds </returns>
        public static long TimeUtc()
        {
            return (long)(DateTime.UtcNow - EpochUTC).TotalSeconds;
        }
        
        /// <summary>
        /// Get time UTC in seconds
        /// </summary>
        /// <param name="dateTime">input date time</param>
        /// <returns></returns>
        public static long TimeUtc(DateTime dateTime)
        {
            return (long)(DateTime.UtcNow - EpochUTC).TotalSeconds;
        }
        
        /// <summary>
        /// Get time UTC in milliseconds
        /// </summary>
        /// <returns> milliseconds </returns>
        public static long MillisecondsUtc()
        {
            return (long) (DateTime.UtcNow - EpochUTC).TotalMilliseconds;
        }
        
        /// <summary>
        /// Get time local in seconds.
        /// </summary>
        /// <returns></returns>
        public static long TimeLocal()
        {
            return (long)(DateTime.Now - EpochLocal).TotalSeconds;
        }


        public static DateTime DateTimeFromSeconds(long seconds)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(seconds);
        }

        public static DateTime LocalDateTimeFromSeconds(long seconds)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local).AddSeconds(seconds);
        }
        
        public static long TimeFromString(string timeString, string format)
        {
            try
            {
                DateTime dt       = DateTime.ParseExact(timeString, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
                long     valueUTC = TimeUtc(dt);
                return valueUTC;
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public static DateTime DateTimeFromString(string timeString, string format)
        {
            try
            {
                DateTime dt = DateTime.ParseExact(timeString, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
                return dt;
            }
            catch (Exception e)
            {
                return DateTime.Now;
            }
        }
        
        public static string GetTimeString(long timeInSeconds, bool noSpace = false)
        {
            if (timeInSeconds > 86400) //convert to day => show day + hour
            {
                int days  = (int)(timeInSeconds / 86400);
                int hours = Mathf.FloorToInt((float)(timeInSeconds - days * 86400) / 3600 /*+ 0.5f*/); //round up final value: 0.5 hour => will show as 1 hour 
                return noSpace ? $"{days}d{hours}h" : $"{days}d {hours}h";
            }
            else if (timeInSeconds > 3600) //convert to hours => show hour + minutes
            {
                int hours   = (int)(timeInSeconds / 3600);
                int minutes = Mathf.FloorToInt(((float)timeInSeconds - hours * 3600) / 60 /*+ 0.5f*/);
                return noSpace ? $"{hours}h{minutes}m" : $"{hours}h {minutes}m";
            }
            else if (timeInSeconds > 60) //convert to minutes => show minutes + seconds
            {
                int minutes = (int)(timeInSeconds / 60);
                int seconds = (int)(timeInSeconds % 60);
                return noSpace ? $"{minutes}m{seconds}s" : $"{minutes}m {seconds}s";
            }
            else
            {
                return noSpace ? $"0m{timeInSeconds}s" : $"0m {timeInSeconds}s";
            }
        }
        
        public static string GetTimerString(long timeInSeconds)
        {
            int munites = (int) (timeInSeconds / 60);
            int seconds = (int) (timeInSeconds % 60);
            return string.Format("{0:D2}:{1:D2}", munites, seconds);
        }

        public static string GetTimerString_H_M_S(long timeInSeconds)
        {
            int hours   = (int) (timeInSeconds / (60 * 60));
            int munites = (int) ((timeInSeconds - hours * (60 * 60)) / 60);
            int seconds = (int) (timeInSeconds % 60);
            return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, munites, seconds);
        }

        public static long ConvertLocalToUTC(long localTimeStamp)
        {
            DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
            date = date.AddSeconds((int) localTimeStamp);
            DateTime nowUTC = TimeZoneInfo.ConvertTimeToUtc(date, TimeZoneInfo.Local);
            return (long) (nowUTC - EpochUTC).TotalSeconds;
        }

        public static int CompareDayVsDay(long timeStamp1, long timeStamp2)
        {
            DateTime date_1 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            date_1 = date_1.AddSeconds((int) timeStamp1);

            DateTime date_2 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            date_2 = date_2.AddSeconds((int) timeStamp2);

            if (date_1.Year > date_2.Year) { return 1; }
            else if (date_1.Year < date_2.Year) { return -1; }
            else
            {
                if (date_1.Month > date_2.Month) { return 1; }
                else if (date_1.Month < date_2.Month) { return -1; }
                else
                {
                    if (date_1.Date > date_2.Date) { return 1; }
                    else if (date_1.Date < date_2.Date) { return -1; }
                    else { return 0; }
                }
            }
        }
    }
}