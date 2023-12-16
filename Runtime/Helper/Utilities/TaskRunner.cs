#region Header
// Date: 17/09/2023
// Created by: Huynh Phong Tran
// File name: TaskRunner.cs
#endregion


using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Base.Logging;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Base.Helper
{
    /// <summary>
    /// Helper class for using async methods returning Tasks from synchronous code and coroutines.
    /// </summary>
    public static class TaskRunner
    {
        private static IDictionary<string, CancellationTokenSource> m_savedCancellationToken = new Dictionary<string, CancellationTokenSource>();

        private static void LazyInitializeCancellationToken(string typeName)
        {
            if (!m_savedCancellationToken.ContainsKey(typeName))
            {
                m_savedCancellationToken.Add(typeName, new CancellationTokenSource());
            }
            else
            {
                m_savedCancellationToken[typeName] = new CancellationTokenSource();
            }
        }

        public static CancellationToken GetCancellationTokenForType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                PDebug.Warn("TaskRunner: Trying to get a UniTask cancellation token. Type name is empty");
                return default;
            }
            
            LazyInitializeCancellationToken(typeName);

            return m_savedCancellationToken[typeName].Token;
        }

        public static void CancelTrackedTask(string typeName)
        {
            if (!m_savedCancellationToken.ContainsKey(typeName))
            {
                return;
            }
            
            m_savedCancellationToken[typeName]?.Cancel();
            m_savedCancellationToken[typeName] = null;
            m_savedCancellationToken.Remove(typeName);
        }

        public static void CancelAllTrackedTask()
        {
            PDebug.Info($"{nameof(TaskRunner)}: Cancel all tracked task");
            foreach (var token in m_savedCancellationToken)
            {
                token.Value?.Cancel();
            }
            
            m_savedCancellationToken.Clear();
        }
        
        private static async UniTask Run(Task task, Action onComplete = null, Action<Exception> onError = null)
        {
            try
            {
                await task;
                onComplete?.Invoke();
            }
            catch (Exception e)
            {
                PDebug.Error(e, "[TaskRunner] Task Run Failed: {0}", e.Message);
                try
                {
                    onError?.Invoke(e);
                }
                catch (Exception callBackException)
                {
                    PDebug.Error(callBackException, "[TaskRunner] Error Callback Exception: {0}", callBackException.Message);
                }
            }
        }
        
        private static async UniTask Run<T>(Task<T> task, Action<T> onComplete = null, Action<Exception> onError = null)
        {
            try
            {
                T result = await task;
                onComplete?.Invoke(result);
            }
            catch (Exception e)
            {
                PDebug.Error(e, "[TaskRunner] Task Run Failed: {0}", e.Message);
                try
                {
                    onError?.Invoke(e);
                }
                catch (Exception callBackException)
                {
                    PDebug.Error(callBackException, "[TaskRunner] Error Callback Exception: {0}", callBackException.Message);
                }
            }
        }
        
        /// <summary>
        /// Yields until the task finishes execution.
        /// If the task throws an exception, the exception is logged and the yielding stops.
        /// </summary>
        /// <param name="task">Function starting the asynchronous method returning the task.</param>
        public static IEnumerator Yield<T>(Func<Task<T>> task)
        {
            UniTask checkedTask = Run(task());
            while (checkedTask.Status is not (UniTaskStatus.Succeeded or UniTaskStatus.Canceled or UniTaskStatus.Faulted) )
            {
                yield return null;
            }
        }
        
        /// <summary>
        /// Yields until the task finishes execution.
        /// If the task throws an exception, the exception is logged and the yielding stops.
        /// </summary>
        /// <param name="task">Function starting the asynchronous method returning the task.</param>
        public static IEnumerator Yield(Func<Task> task)
        {
            UniTask checkedTask = Run(task());
            while (checkedTask.Status is not (UniTaskStatus.Succeeded or UniTaskStatus.Canceled or UniTaskStatus.Faulted) )
            {
                yield return null;
            }
        }

        public static void Start(Task task, Action onComplete = null, Action<Exception> onError = null)
        {
            Run(task, onComplete, onError).Forget();
        }

        public static void Start<T>(Task<T> task, Action<T> onComplete = null, Action<Exception> onError = null)
        {
            Run(task, onComplete, onError).Forget();
        }

        public static UniTask WaitUntil(string callerName, Func<bool> predicate,  PlayerLoopTiming timing = PlayerLoopTiming.Update)
        {
            return UniTask.WaitUntil(predicate, timing, GetCancellationTokenForType(callerName));
        }

        public static UniTask WaitUntil(Func<bool> predicate, CancellationToken ct, PlayerLoopTiming timing = PlayerLoopTiming.Update)
        {
            return UniTask.WaitUntil(predicate, timing, ct);
        }
        
        public static UniTask WaitUntil(Func<bool> predicate, PlayerLoopTiming timing = PlayerLoopTiming.Update)
        {
            return UniTask.WaitUntil(predicate, timing);
        }

        public static UniTask WaitWhile(string callerName, Func<bool> predicate, PlayerLoopTiming timing = PlayerLoopTiming.Update)
        {
            return UniTask.WaitWhile(predicate, timing, GetCancellationTokenForType(callerName));
        }
        
        public static UniTask WaitWhile(Func<bool> predicate, CancellationToken ct, PlayerLoopTiming timing = PlayerLoopTiming.Update)
        {
            return UniTask.WaitWhile(predicate, timing, ct);
        }
        
        public static UniTask WaitWhile(Func<bool> predicate, PlayerLoopTiming timing = PlayerLoopTiming.Update)
        {
            return UniTask.WaitWhile(predicate, timing);
        }
        
    }
}