using System.Collections;
using UnityEngine;
using System;
using System.Collections.Concurrent;
using Base.Core;
using Base.Helper;

public interface IMainThreadDispatcher : IService
{
    /// <summary>
    /// Locks the queue and adds the IEnumerator to the queue
    /// </summary>
    /// <param name="action">IEnumerator function that will be executed from the main thread.</param>
    void Dispatch(IEnumerator action);

    /// <summary>
    /// Locks the queue and adds the Action to the queue
    /// </summary>
    /// <param name="action">function that will be executed from the main thread.</param>
    void Dispatch(Action action);

    /// <summary>
    /// Dispatch
    /// </summary>
    /// <param name="delay"></param>
    /// <param name="action"></param>
    void Dispatch(float delay, Action action);
}

public class UnityMainThreadDispatcher : BaseMono
{
    readonly ConcurrentQueue<Action> _executionQueue = new ConcurrentQueue<Action>();

    public bool IsInitialize { get; private set; }

    void Update()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                try
                {
                    if (_executionQueue.TryDequeue(out Action action))
                    {
                        action.Invoke();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
    }

    /// <summary>
    /// Locks the queue and adds the IEnumerator to the queue
    /// </summary>
    /// <param name="action">IEnumerator function that will be executed from the main thread.</param>
    public void Dispatch(IEnumerator action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(() =>
                                    {
                                        StartCoroutine(action);
                                    });
        }
    }

    /// <summary>
    /// Locks the queue and adds the Action to the queue
    /// </summary>
    /// <param name="action">function that will be executed from the main thread.</param>
    public void Dispatch(Action action)
    {
        Dispatch(ActionWrapper(action));
    }
    IEnumerator ActionWrapper(Action a)
    {
        a?.Invoke();
        yield return null;
    }

    /// <summary>
    /// Locks the queue and adds the Action to the queue
    /// </summary>
    /// <param name="delay"></param>
    /// <param name="action"></param>
    public void Dispatch(float delay, Action action)
    {
        Dispatch(ActionWrapper(delay, action));
    }
    IEnumerator ActionWrapper(float delay, Action a)
    {
        if (delay > 0) { yield return new WaitForSeconds(delay); }
        a?.Invoke();
        yield return null;
    }

    /// <summary>
    /// Locks the queue and adds the Action to the queue
    /// </summary>
    /// <param name="delay"></param>
    /// <param name="action"></param>
    public void Dispatch(float delay, Action action, Func<IEnumerator> waitFor)
    {
        Dispatch(ActionWrapper(delay, action, waitFor));
    }
    IEnumerator ActionWrapper(float delay, Action a, Func<IEnumerator> waitFor)
    {
        if (delay > 0) { yield return new WaitForSeconds(delay); }
        if (waitFor != null) { yield return waitFor(); }
        a?.Invoke();
        yield return null;
    }

    protected override void Start()
    {
        BaseContextRegistry.TryGetOrCreateContext(0).Register(this);
    }

    void OnDestroy()
    {
        BaseContextRegistry.TryGetOrCreateContext(0).UnRegister(this);
    }
}
