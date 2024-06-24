using System;
using System.Threading;
using Base.Logging;
using UniRx;

namespace Base.Helper
{
    public class BaseInterval : SingletonNonMono<BaseInterval>
    {
        private CompositeDisposable m_compositeDisposable;

        private CompositeDisposable CompositeDisposable => LazyInitializer.EnsureInitialized(ref m_compositeDisposable);
        
        /// <summary>
        /// Run an action on MainThread every amount of seconds
        /// </summary>
        /// <param name="timeInSeconds"></param>
        /// <param name="callBack"></param>
        public static IDisposable RunInterval(int timeInSeconds, Action callBack = null)
        {
            IDisposable disposable = Observable.Interval(TimeSpan.FromSeconds(timeInSeconds)).Subscribe(_ => callBack?.Invoke(), e => Instance
                                                                                                                                        .OnError(e));
            
            Instance.CompositeDisposable.Add(disposable);
            return disposable;
        }
        /// <summary>
        /// Run an action on MainThread every amount of seconds
        /// </summary>
        /// <param name="timeInSeconds"></param>
        /// <param name="callback"></param>
        public static IDisposable RunInterval(int timeInSeconds, Action<long> callback = null)
        {
            return Observable.Interval(TimeSpan.FromSeconds(timeInSeconds), Scheduler.MainThread)
                                               .Subscribe(unit =>
                                                          {
                                                              callback?.Invoke(unit);
                                                          }, Instance.OnError)
                                               .AddTo(Instance.CompositeDisposable);

        }
        
        /// <summary>
        /// Run an action on MainThread every amount of seconds
        /// </summary>
        /// <param name="dueTime">Time delay at start</param>
        /// <param name="interval">Time delay every frame</param>
        /// <param name="callback">Action</param>
        public static IDisposable RunInterval(float dueTime, int interval, Action callback = null)
        {
            return Observable.Timer(TimeSpan.FromSeconds(dueTime), TimeSpan.FromSeconds(interval))
                                               .Subscribe(_ => callback?.Invoke(), e => Instance.OnError(e))
                                               .AddTo(Instance.CompositeDisposable);
        }
        
        /// <summary>
        /// Run an action on MainThread every amount of second
        /// </summary>
        /// <param name="timeInSeconds"></param>
        /// <param name="numberOfTimeRun">Time limit run</param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IDisposable RunInterval(int timeInSeconds, int numberOfTimeRun, Action callback = null)
        {
            return Observable.Interval(TimeSpan.FromSeconds(timeInSeconds)).Take(numberOfTimeRun)
                .Subscribe(_ => callback?.Invoke(), e => Instance.OnError(e))
                .AddTo(Instance.CompositeDisposable);
        }
        
        /// <summary>
        /// Run action after an amount of time
        /// </summary>
        /// <param name="dueTime">Time delay at start</param>
        /// <param name="startAction"></param>
        /// <param name="scheduler">Can be schedule on MainThread, MainThreadEndOfFrame, ...</param>
        public static IDisposable RunAfterTime(int dueTime, Action startAction = null, IScheduler scheduler = null)
        {
            scheduler ??= Scheduler.MainThread;

            IDisposable disposable = Observable.Timer(TimeSpan.FromSeconds(dueTime), scheduler)
                                               .Subscribe(_ => startAction?.Invoke(), e => Instance.OnError(e));
            Instance.CompositeDisposable.Add(disposable);

            return disposable;
        }

        private void OnError(Exception exception)
        {
            PDebug.Error(exception, "[BaseInterval] Exception ERROR: {0}", exception.Message);
        }

        public static void Cancel()
        {
            Instance.CompositeDisposable.Clear();
        }

        public static void StopInterval(IDisposable disposable)
        {
            Instance.CompositeDisposable.Remove(disposable);
        }
    }
}

