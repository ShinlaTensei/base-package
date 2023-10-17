#region Header
// Date: 04/07/2023
// Created by: Huynh Phong Tran
// File name: InputLock.cs
#endregion

using System.Collections.Generic;
using Base.Pattern;
using Base.Services;
using System;
using Base.Logging;

namespace Base.Module
{
    public class OnForceSetTouch : Signal<bool>
    {
    }

    public static class InputLocker
    {
    #if LOG_ENABLE
        public static bool LOG_ENABLE = false;
    #else
        public static bool LOG_ENABLE = false;
    #endif

        public static bool HasIntros     { get { return s_introLockType.Count > 0; } }
        public static bool HasOutros     { get { return s_outroLockType.Count > 0; } }
        public static bool HasLoading    { get { return s_loadingLockType.Count > 0; } }
        public static bool HasLockedAPIs { get { return s_apiLockType.Count > 0; } }
        public static bool HasFreeLocked { get { return s_freeLockType.Count > 0; } }
        public static bool HasFTUELocked { get { return s_ftueLock.Count > 0; } }
        public static bool IsInputLocked { get { return !ForceEnable && (HasLoading || HasLockedAPIs || HasFreeLocked || HasFTUELocked || HasIntros || HasOutros); } }
        public static bool ForceEnable   { get; set; }

        /// <summary>
        /// reset
        /// </summary>
        public static void Reset()
        {
            s_introLockType.Clear();
            s_outroLockType.Clear();
            s_loadingLockType.Clear();
            s_apiLockType.Clear();
            s_ftueLock.Clear();
            s_freeLockType.Clear();
        }

        /// <summary>
        /// Debug locked type
        /// </summary>
        /// <param name="forceShow"></param>
        public static void DebugLockedType(bool forceShow = false)
        {
            if (LOG_ENABLE || forceShow)
            {
                string s = $"IntroLockCount:{s_introLockType.Count} OutroLockCount:{s_outroLockType.Count} LoadingLockCount:{s_loadingLockType.Count} APILockCount:{s_apiLockType.Count} FreeLockCount:{s_freeLockType.Count}";
                string data  = string.Empty;
                {
                    List<string> inputLockTypes = new List<string>();
                    for (int index = 0; index < s_introLockType.Count; index++) { inputLockTypes.Add(s_introLockType[index].Name); }
                    if (inputLockTypes.Count > 0)
                    {
                        data += $"{(!string.IsNullOrEmpty(data) ? "\n" : "")}Intro:'{string.Join(",", inputLockTypes.ToArray())}'";
                    }
                }
                {
                    List<string> inputLockTypes = new List<string>();
                    for (int index = 0; index < s_outroLockType.Count; index++) { inputLockTypes.Add(s_outroLockType[index].Name); }
                    if (inputLockTypes.Count > 0)
                    {
                        data += $"{(!string.IsNullOrEmpty(data) ? "\n" : "")}Outro:'{string.Join(",", inputLockTypes.ToArray())}'";
                    }
                }
                {
                    List<string> pendingLockTypes = new List<string>();
                    for (int index = 0; index < s_loadingLockType.Count; index++) { pendingLockTypes.Add(s_loadingLockType[index].Name); }
                    if (pendingLockTypes.Count > 0)
                    {
                        data += $"{(!string.IsNullOrEmpty(data) ? "\n" : "")}Loading:'{string.Join(",", pendingLockTypes.ToArray())}'";
                    }
                }
                {
                    List<string> apiLockTypes = new List<string>();
                    for (int index = 0; index < s_apiLockType.Count; index++) { apiLockTypes.Add(s_apiLockType[index].Name); }
                    if (apiLockTypes.Count > 0)
                    {
                        data += $"{(!string.IsNullOrEmpty(data) ? "\n" : "")}API:'{string.Join(",", apiLockTypes.ToArray())}'";
                    }
                }
                {
                    if (s_ftueLock.Count > 0)
                    {
                        data += $"{(!string.IsNullOrEmpty(data) ? "\n" : "")}FTUE:'{string.Join(",", s_ftueLock.ToArray())}'";
                    }
                }
                {
                    if (s_freeLockType.Count > 0)
                    {
                        data += $"{(!string.IsNullOrEmpty(data) ? "\n" : "")}Free:'{string.Join(",", s_freeLockType.ToArray())}'";
                    }
                }

                PDebug.DebugFormat("{0} - {1}", s, data);
            }
        }

        /// <summary>
        /// Input locker
        /// </summary>
        static InputLocker()
        {
            ServiceLocator.Get<OnForceSetTouch>()?.Subscribe(ForceSetTouch);
        }

        /// <summary>
        /// Force set touch
        /// </summary>
        /// <param name="value"></param>
        static void ForceSetTouch(bool value)
        {
            ForceEnable = value;
        }

    #region TOUCH PROCESS
        static List<Type> s_introLockType = new List<Type>();
        /// <summary>
        /// intro lock add
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void IntroLockAdd<T>()
        {
            IntroLockAdd(typeof(T));
        }
        /// <summary>
        /// intro lock remove
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void IntroLockRemove<T>()
        {
            IntroLockRemove(typeof(T));
        }
        /// <summary>
        /// intro lock add
        /// </summary>
        /// <param name="t"></param>
        public static void IntroLockAdd(Type t)
        {
            s_introLockType.Add(t);
            if (LOG_ENABLE)
            {
                PDebug.DebugFormat("IntroLockAdd:{0}", t.Name);
                DebugLockedType();
            }
        }
        /// <summary>
        /// intro lock remove
        /// </summary>
        /// <param name="t"></param>
        public static void IntroLockRemove(Type t)
        {
            s_introLockType.Remove(t);
            if (LOG_ENABLE)
            {
                PDebug.DebugFormat("IntroLockRemove:{0}", t.Name);
                DebugLockedType();
            }
        }


        static List<Type> s_outroLockType = new List<Type>();
        /// <summary>
        /// outro lock add
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void OutroLockAdd<T>()
        {
            IntroLockAdd(typeof(T));
        }
        /// <summary>
        /// outro lock remove
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void OutroLockRemove<T>()
        {
            IntroLockRemove(typeof(T));
        }
        /// <summary>
        /// outro lock add
        /// </summary>
        /// <param name="t"></param>
        public static void OutroLockAdd(Type t)
        {
            s_outroLockType.Add(t);
            if (LOG_ENABLE)
            {
                PDebug.DebugFormat("OutroLockAdd:{0}", t.Name);
                DebugLockedType();
            }
        }
        /// <summary>
        /// outro lock remove
        /// </summary>
        /// <param name="t"></param>
        public static void OutroLockRemove(Type t)
        {
            s_outroLockType.Remove(t);
            if (LOG_ENABLE)
            {
                PDebug.DebugFormat("OutroLockRemove:{0}", t.Name);
                DebugLockedType();
            }
        }

        static List<Type> s_loadingLockType = new List<Type>();
        /// <summary>
        /// loading lock add
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void LoadingLockAdd<T>()
        {
            LoadingLockAdd(typeof(T));
        }
        /// <summary>
        /// loading lock remove
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void LoadingLockRemove<T>()
        {
            LoadingLockRemove(typeof(T));
        }
        /// <summary>
        /// loading lock add
        /// </summary>
        /// <param name="t"></param>
        public static void LoadingLockAdd(Type t)
        {
            s_loadingLockType.Add(t);
            if (LOG_ENABLE)
            {
                PDebug.DebugFormat("LoadingLockAdd:{0}", t.Name);
                DebugLockedType();
            }
        }
        /// <summary>
        /// loading lock remove
        /// </summary>
        /// <param name="t"></param>
        public static void LoadingLockRemove(Type t)
        {
            s_loadingLockType.Remove(t);
            if (LOG_ENABLE)
            {
                PDebug.DebugFormat("LoadingLockRemove:{0}", t.Name);
                DebugLockedType();
            }
        }
    #endregion

    #region PendingAPIs
        static List<Type> s_apiLockType = new List<Type>();
        /// <summary>
        /// API lock add
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void APILockAdd<T>()
        {
            APILockAdd(typeof(T));
        }
        /// <summary>
        /// API lock remove
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void APILockRemove<T>()
        {
            APILockRemove(typeof(T));
        }
        /// <summary>
        /// API lock add
        /// </summary>
        /// <param name="t"></param>
        public static void APILockAdd(Type t)
        {
            s_apiLockType.Add(t);
            if (LOG_ENABLE)
            {
                PDebug.DebugFormat("APILockAdd:{0}", t.Name);
                DebugLockedType();
            }
        }
        /// <summary>
        /// API lock remove
        /// </summary>
        /// <param name="t"></param>
        public static void APILockRemove(Type t)
        {
            s_apiLockType.Remove(t);
            if (LOG_ENABLE)
            {
                PDebug.DebugFormat("APILockRemove:{0}", t.Name);
                DebugLockedType();
            }
        }
    #endregion

    #region FTUELock
        static List<string> s_ftueLock = new List<string>();
        /// <summary>
        /// FTUE lock add
        /// </summary>
        /// <param name="t"></param>
        public static void FTUELockAdd(string t)
        {
            s_ftueLock.Add(t);

            if (LOG_ENABLE)
            {
                PDebug.DebugFormat("FTUELockAdd:{0}", t);
                DebugLockedType();
            }
        }
        /// <summary>
        /// FTUE lock remove
        /// </summary>
        /// <param name="t"></param>
        public static void FTUELockRemove(string t)
        {
            s_ftueLock.Remove(t);
            if (LOG_ENABLE)
            {
                PDebug.DebugFormat("FTUELockRemove:{0}", t);
                DebugLockedType();
            }
        }
    #endregion
        
    #region FreeLock
        static List<string> s_freeLockType = new List<string>();
        /// <summary>
        /// Free lock add
        /// </summary>
        /// <param name="t"></param>
        public static void FreeLockAdd(string t)
        {
            s_freeLockType.Add(t);
            
            if (LOG_ENABLE)
            {
                PDebug.DebugFormat("FreeLockAdd:{0}", t);
                DebugLockedType();
            }
        }
        /// <summary>
        /// Free lock remove
        /// </summary>
        /// <param name="t"></param>
        public static void FreeLockRemove(string t)
        {
            s_freeLockType.Remove(t);
            
            if (LOG_ENABLE)
            {
                PDebug.DebugFormat("FreeLockRemove:{0}", t);
                DebugLockedType();
            }
        }
        /// <summary>
        /// Check if has free lock
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool HasFreeLock(string t)
        {
            return s_freeLockType.Contains(t);
        }
    #endregion
    }
}