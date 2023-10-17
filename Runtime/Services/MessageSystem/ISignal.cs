using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Base.Logging;
using NLog;
using UnityEngine;

namespace Base.Services
{
    public interface ISignal
    {
        void RemoveAllListener();
    }

    public class Signal : ISignal
    {
        private event Action Listener = null;
        private event Action OnceListener = null;
        public void RemoveAllListener()
        {
            Listener = null;
            OnceListener = null;
        }

        public void Subscribe(Action callback, bool isOnce = false)
        {
            if (isOnce)
                OnceListener = AddUnique(OnceListener, callback);
            else
                Listener = AddUnique(Listener, callback);
        }

        public void UnSubscribe(Action callback)
        { 
            if (Listener != null)
                Listener -= callback;
        }

        public void Dispatch()
        {
            try
            {
                Listener?.Invoke();
            }
            catch (Exception e)
            {
                PDebug.GetLogger().Error(e, e.Message);
            }

            try
            {
                OnceListener?.Invoke();
            }
            catch (Exception e)
            {
                PDebug.GetLogger().Error(e, e.Message);
            }

            OnceListener = null;
        }

        private Action AddUnique(Action listener, Action callback)
        {
            if (listener == null || !listener.GetInvocationList().Contains(callback))
            {
                listener += callback;
            }

            return listener;
        }
    }

    public class Signal<T> : ISignal
    {
        private event Action<T> Listener = null;
        private event Action<T> OnceListener = null;
        public void Subscribe(Action<T> callback, bool isOnce = false)
        {
            if (isOnce) OnceListener = AddUnique(OnceListener, callback);
            else Listener = AddUnique(Listener, callback);
        }

        public void UnSubscribe(Action<T> callback)
        {
            if (Listener != null)
                Listener -= callback;
        }
        
        public void Dispatch(T arg)
        {
            try
            {
                Listener?.Invoke(arg);
            }
            catch (Exception e)
            {
                PDebug.GetLogger().Error(e, e.Message);
            }

            try
            {
                OnceListener?.Invoke(arg);
            }
            catch (Exception e)
            {
                PDebug.GetLogger().Error(e, e.Message);
            }

            OnceListener = null;
        }
        public void RemoveAllListener()
        {
            Listener = null;
            OnceListener = null;
        }
        
        private Action<T> AddUnique(Action<T> listener, Action<T> callback)
        {
            if (listener == null || !listener.GetInvocationList().Contains(callback))
            {
                listener += callback;
            }

            return listener;
        }
    }

    public class Signal<T, V> : ISignal
    {
        private event Action<T, V> Listener = null;
        private event Action<T, V> OnceListener = null;
        public void Subscribe(Action<T, V> callback, bool isOnce = false)
        {
            if (isOnce) OnceListener = AddUnique(OnceListener, callback);
            else Listener = AddUnique(Listener, callback);
        }

        public void UnSubscribe(Action<T, V> callback)
        {
            if (Listener != null)
                Listener -= callback;
        }
        
        public void Dispatch(T arg1, V arg2)
        {
            try
            {
                Listener?.Invoke(arg1, arg2);
            }
            catch (Exception e)
            {
                PDebug.GetLogger().Error(e, e.Message);
            }

            try
            {
                OnceListener?.Invoke(arg1, arg2);
            }
            catch (Exception e)
            {
                PDebug.GetLogger().Error(e, e.Message);
            }

            OnceListener = null;
        }
        public void RemoveAllListener()
        {
            Listener = null;
            OnceListener = null;
        }
        
        private Action<T, V> AddUnique(Action<T, V> listener, Action<T, V> callback)
        {
            if (listener == null || !listener.GetInvocationList().Contains(callback))
            {
                listener += callback;
            }

            return listener;
        }
    }

    public class Signal<T, V, U> : ISignal
    {
        private event Action<T, V, U> Listener = null;
        private event Action<T, V, U> OnceListener = null;
        public void Subscribe(Action<T, V, U> callback, bool isOnce = false)
        {
            if (isOnce) OnceListener = AddUnique(OnceListener, callback);
            else Listener = AddUnique(Listener, callback);
        }

        public void UnSubscribe(Action<T, V, U> callback)
        {
            if (Listener != null)
                Listener -= callback;
        }
        
        public void Dispatch(T arg1, V arg2, U arg3)
        {
            try
            {
                Listener?.Invoke(arg1, arg2, arg3);
            }
            catch (Exception e)
            {
                PDebug.GetLogger().Error(e);
            }

            try
            {
                OnceListener?.Invoke(arg1, arg2, arg3);
            }
            catch (Exception e)
            {
                PDebug.GetLogger().Error(e);
            }

            OnceListener = null;
        }
        public void RemoveAllListener()
        {
            Listener = null;
            OnceListener = null;
        }
        
        private Action<T, V ,U> AddUnique(Action<T, V, U> listener, Action<T, V, U> callback)
        {
            if (listener == null || !listener.GetInvocationList().Contains(callback))
            {
                listener += callback;
            }

            return listener;
        }
    }
}
