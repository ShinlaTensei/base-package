using Base.Core;
using UnityEngine;

namespace Base
{
    public partial class PermissionService : Service
    {
        private AndroidJavaObject m_userPermission;

        private AndroidJavaObject GetUnityPermission()
        {
            if (m_userPermission != null)
            {
                return m_userPermission;
            }

            m_userPermission = (AndroidJavaObject)new AndroidJavaClass("com.unity3d.player.UnityPermissions");
            return m_userPermission;
        }

        public override void Dispose()
        {
            m_userPermission.Dispose();
        }
    }
    
    public partial class PermissionService
    {
        public const string CAMERA = "android.permission.CAMERA";
        public const string MICROPHONE = "android.permission.RECORD_AUDIO";
        public const string FINE_LOCATION = "android.permission.ACCESS_FINE_LOCATION";
        public const string COARSE_LOCATION = "android.permission.ACCESS_COARSE_LOCATION";
        public const string EXTERNAL_STORAGE_READ = "android.permission.READ_EXTERNAL_STORAGE";
        public const string EXTERNAL_STORAGE_WRITE = "android.permission.WRITE_EXTERNAL_STORAGE";
        public const string WIFI_STATE = "android.permission.ACCESS_WIFI_STATE";
        public const string NETWORK_STATE = "androdi.permission.ACCESS_NETWORK_STATE";
        public const string VIBRATE = "android.permission.VIBRATE";
    }
}