#region Header
// Date: 17/09/2023
// Created by: Huynh Phong Tran
// File name: TaskRunner.cs
#endregion


using System;
using System.Collections;
using System.Threading.Tasks;
using Base.Logging;
using Cysharp.Threading.Tasks;

namespace Base.Helper
{
    /// <summary>
    /// Helper class for using async methods returning Tasks from synchronous code and coroutines.
    /// </summary>
    public static class TaskRunner
    {
        private static async UniTask Run(Task task, Action onComplete = null, Action<Exception> onError = null)
        {
            try
            {
                await task;
                onComplete?.Invoke();
            }
            catch (Exception e)
            {
                PDebug.Error(e, "[TaskRunner]");
                try
                {
                    onError?.Invoke(e);
                }
                catch (Exception callBackException)
                {
                    PDebug.Error(callBackException, "[TaskRunner] Error Callback Exception");
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
                PDebug.Error(e, "[TaskRunner]");
                try
                {
                    onError?.Invoke(e);
                }
                catch (Exception callBackException)
                {
                    PDebug.Error(callBackException, "[TaskRunner] Error Callback Exception");
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
    }
}