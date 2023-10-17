using NLog;
using NLog.Targets;
using UnityEngine;

namespace Base.Logging
{
    [Target("UnityDebugLog")]
    public class UnityDebugTarget : TargetWithContext
    {
        protected override void InitializeTarget()
        {
            base.InitializeTarget();
        }

        protected override void Write(LogEventInfo logEvent)
        {
            string logMessage = RenderLogEvent(this.Layout, logEvent);
            string[] split = logMessage.Split("---");
            string level = split.Length > 0 ? split[0] : string.Empty;
            string message = split.Length > 1 ? split[1] : string.Empty;
            string stackTrace = split.Length > 2 ? split[2] : string.Empty;
            if (logEvent.Level <= LogLevel.Info)
                Debug.LogFormat("<b><color=aqua>{0}</color></b> --- <b>{1}</b> --- {2}", level, message, stackTrace);
            else if (logEvent.Level == LogLevel.Warn)
                Debug.LogWarningFormat("<b><color=yellow>{0}</color></b> --- <b>{1}</b> --- {2}", level, message, stackTrace);
            else
                Debug.LogErrorFormat("<b><color=red>{0}</color></b> --- <b>{1}</b> --- {2}", level, message, stackTrace);
        }
    }
}

