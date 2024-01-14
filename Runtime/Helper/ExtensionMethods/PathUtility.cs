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
            while (path1.EndsWith("/") || path1.EndsWith("\\"))
            {
                path1 = path1.Substring(0, path1.Length - 1);
            }
            while (path2.StartsWith("/") || path2.StartsWith("\\"))
            {
                path1 = path1.Substring(1);
            }
            return path1 + "/" + path2;
        }
    }
}