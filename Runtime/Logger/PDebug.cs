using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Base.Module;
using NLog;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Logger = NLog.Logger;

namespace Base.Logging
{
    public static class PDebug
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void SetupLogSystem()
        {
            // Init configuration
            var config = new NLog.Config.LoggingConfiguration();
            
            var logConsole = new UnityDebugTarget()
            {
                Name = "UnityDebugLog",
                Layout = "[${level}]---${message}---${callsite:captureStackTrace=true:skipFrames=1:fileName=true}"
            };
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logConsole);

            LogManager.Configuration = config;
            
#if !LOG_ENABLE
            LogManager.Shutdown();
#endif
        }

        public static Logger GetLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        #region Log function

        public static void Info(string message)
        {
            if (GetLogger().IsInfoEnabled) GetLogger().Info(message);
        }

        public static void Debug(string message)
        {
            if (GetLogger().IsDebugEnabled) GetLogger().Debug(message);
        }

        public static void Warn(string message)
        {
            if (GetLogger().IsWarnEnabled) GetLogger().Warn(message);
        }

        public static void Error(string message)
        {
            if (GetLogger().IsErrorEnabled) GetLogger().Error(message);
        }

        public static void Info(Color color, string message)
        {
            if (GetLogger().IsInfoEnabled) Info(Format(null, "<color=#{0}>{1}</color>", ToHtmlStringRGBA(color), message));
        }
        
        public static void Debug(Color color, string message)
        {
            if (GetLogger().IsDebugEnabled) Debug(Format(null, "<color=#{0}>{1}</color>", ToHtmlStringRGBA(color), message));
        }
        
        public static void Warn(Color color, string message)
        {
            if (GetLogger().IsWarnEnabled) Warn(Format(null, "<color=#{0}>{1}</color>", ToHtmlStringRGBA(color), message));
        }
        
        public static void Error(Color color, string message)
        {
            if (GetLogger().IsErrorEnabled) Error(Format(null, "<color=#{0}>{1}</color>", ToHtmlStringRGBA(color), message));
        }

        public static void InfoFormat(string format, params object[] args)
        {
            GetLogger().Info(format, args);
        }
        
        public static void DebugFormat(string format, params object[] args)
        {
            GetLogger().Debug(format, args);
        }
        
        public static void WarnFormat(string format, params object[] args)
        {
            GetLogger().Warn(format, args);
        }
        
        public static void ErrorFormat(string format, params object[] args)
        {
            GetLogger().Error(format, args);
        }

        public static void InfoFormat(Color color, string format, params object[] args)
        {
            InfoFormat(Format(null, "<color=#{0}>{1}</color> ", ToHtmlStringRGBA(color), format), args);
        }
        
        public static void DebugFormat(Color color, string format, params object[] args)
        {
            DebugFormat(Format(null, "<color=#{0}>{1}</color> ", ToHtmlStringRGBA(color), format), args);
        }
        
        public static void WarnFormat(Color color, string format, params object[] args)
        {
            WarnFormat(Format(null, "<color=#{0}>{1}</color> ", ToHtmlStringRGBA(color), format), args);
        }
        
        public static void ErrorFormat(Color color, string format, params object[] args)
        {
            ErrorFormat(Format(null, "<color=#{0}>{1}</color> ", ToHtmlStringRGBA(color), format), args);
        }

        public static void Error(Exception exception, string message, params object[] args)
        {
            GetLogger().Error(exception, message, args);
        }

        #endregion

        private static string ToHtmlStringRGBA(Color color)
        {
            Color32 color32 = new Color32((byte) Mathf.Clamp(Mathf.RoundToInt(color.r * (float) byte.MaxValue), 0, (int) byte.MaxValue), (byte) Mathf.Clamp(Mathf.RoundToInt(color.g * (float) byte.MaxValue), 0, (int) byte.MaxValue), (byte) Mathf.Clamp(Mathf.RoundToInt(color.b * (float) byte.MaxValue), 0, (int) byte.MaxValue), (byte) Mathf.Clamp(Mathf.RoundToInt(color.a * (float) byte.MaxValue), 0, (int) byte.MaxValue));
            return Format((IFormatProvider) null, "{0:X2}{1:X2}{2:X2}{3:X2}", (object) color32.r, (object) color32.g, (object) color32.b, (object) color32.a);
        }
        
        private static string Format(IFormatProvider provider, string format, params object[] args)
        {
            if (format == null)
                return (string) null;
            if (args == null)
                throw new ArgumentNullException(nameof (args));
            StringBuilder stringBuilder = new StringBuilder(format.Length + args.Length * 8);
            stringBuilder.AppendFormat(provider, format, args);
            return stringBuilder.ToString();
        }
    }
}
