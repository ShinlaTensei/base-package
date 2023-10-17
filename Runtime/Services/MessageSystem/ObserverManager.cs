using System;
using System.Collections.Generic;
using Base.Pattern;
using UnityEngine;

namespace Base.Services
{
    public class ObserverManager : Service, IDisposable
    {
        private Dictionary<Enum, Callback<object>> _listeners;

        public void AddListener(Enum eventId, Callback<object> func)
        {
            Debug.AssertFormat(func != null, "AddListener event {0} failed due to callback is null !!!", new object[] {eventId.ToString()});
            if (_listeners.ContainsKey(eventId))
            {
                _listeners[eventId] += func;
            }
            else
            {
                if (_listeners.TryAdd(eventId, func))
                {
                    
                }
                else
                {
                    _listeners.Add(eventId, null);
                    _listeners[eventId] = func;
                }
            }
        }

        public void RemoveListener(Enum eventId, Callback<object> func)
        {
            Debug.AssertFormat(func != null, "RemoveListener event {0} failed due to callback is null !!!", new object[] {eventId.ToString()});

            if (_listeners.ContainsKey(eventId))
            {
                _listeners[eventId] -= func;
            }
            else
            {
                Debug.LogErrorFormat("Event {0} not found", new object[] {eventId.ToString()});
            }
        }

        public void BroadcastEvent(Enum eventId, object argument)
        {
            if (!_listeners.ContainsKey(eventId))
            {
                Debug.LogErrorFormat("No one subscribe for event {0}", new object[] {eventId.ToString()});
                return;
            }

            var callback = _listeners[eventId];
            if (callback != null)
            {
                callback.Invoke(argument);
            }
            else
            {
                Debug.LogErrorFormat("Event {0} has zero listener", new object[] {eventId.ToString()});
                _listeners.Remove(eventId);
            }
        }

        private void ReleaseUnmanagedResources()
        {
            // TODO release unmanaged resources here
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                _listeners.Clear();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ObserverManager()
        {
            Dispose();
        }

        public void Init()
        {
            _listeners = new Dictionary<Enum, Callback<object>>();
        }

        public void DeInit()
        {
            
        }
    }

    public static class ObserverExtension
    {
        public static void RegisterListener(this MonoBehaviour target, Enum eventId, Callback<object> func)
        {
            ServiceLocator.Get<ObserverManager>().AddListener(eventId, func);
        }

        public static void RemoveListener(this MonoBehaviour target, Enum eventId, Callback<object> func)
        {
            try
            {
                ServiceLocator.Get<ObserverManager>().RemoveListener(eventId, func);
            }
            catch (Exception e)
            {
                Debug.LogException(exception: e);
            }
        }

        public static void PostEvent(this MonoBehaviour target, Enum eventId, object argument)
        {
            ServiceLocator.Get<ObserverManager>().BroadcastEvent(eventId, argument);
        }
    }
}
