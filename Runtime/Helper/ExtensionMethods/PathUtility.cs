using System;
using System.IO;
using UnityEngine;

namespace Base.Helper
{
    public static class PathUtility
    {
        private static string m_projectPath = string.Empty;
        
        public static string GetProjectPath()
        {
            if (!string.IsNullOrEmpty(m_projectPath))
            {
                return m_projectPath;
            }

            string dataPath = Application.dataPath;
            DirectoryInfo parentDir = Directory.GetParent(dataPath);
            m_projectPath = parentDir?.FullName ?? string.Empty;

            return m_projectPath;
        }
        
        /// <summary>
        /// Get the system path based on platform
        /// </summary>
        /// <returns>The path specific on each platform
        /// (Window:"C:\Users\{Your_user_name}\", Android: "/storage/emulated/0/Android/data/{your_package_name}/files/")</returns>
        public static string GetSystemPath()
        {
#if (UNITY_EDITOR || UNITY_STANDALONE || UNITY_STANDALONE_WIN) && !UNITY_EDITOR_OSX
            return Environment.GetEnvironmentVariable("USERPROFILE") + Path.DirectorySeparatorChar;
#elif UNITY_ANDROID || !DEBUG || UNITY_EDITOR_OSX
            return Application.persistentDataPath + Path.DirectorySeparatorChar;
#endif
        }
        
        public static string TrimSlashes(string path)
        {
            path = TrimLeadingSlashes(path);
            path = TrimTailingSlashes(path);
            return path;
        }
        
        public static string TrimLeadingSlashes(string path)
        {
            while (path.StartsWith("/") || path.StartsWith("\\"))
            {
                path = path.Substring(1);
            }

            return path;
        }
        
        public static string TrimTailingSlashes(string path)
        {
            while (path.EndsWith("/") || path.EndsWith("\\"))
            {
                path = path.Substring(0, path.Length - 1);
            }

            return path;
        }
        
        public static string Combine(string path1, string path2, string path3)
        {
            return Combine(Combine(path1, path2), path3);
        }

        /// Combines path1 and path2 with a forward slash '/'
        public static string Combine(string path1, string path2)
        {
            while (path1.EndsWith(Path.DirectorySeparatorChar) || path1.EndsWith(Path.AltDirectorySeparatorChar))
            {
                path1 = path1.Substring(0, path1.Length - 1);
            }
            while (path2.StartsWith(Path.DirectorySeparatorChar) || path2.StartsWith(Path.AltDirectorySeparatorChar))
            {
                path1 = path1.Substring(1);
            }
            return path1 + Path.DirectorySeparatorChar + path2;
        }
        
        public static void CreateFolder(string folderRelativePath)
        {
            string path = PathUtility.GetSystemPath() + folderRelativePath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        
        public static void CreateFolderInProject(string folderRelativePath)
        {
            string path = Combine(GetProjectPath(), folderRelativePath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static void CreateFolder(string directory, string folderName)
        {
            string path = directory + folderName;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}