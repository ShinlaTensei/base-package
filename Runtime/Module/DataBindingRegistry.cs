using System;
using System.Collections.Generic;
using Base.Core;
using Base.Helper;
using Base.Logging;
using UniRx;

namespace Base.Module
{
    public interface IDataInlet<in T> : IDisposable
    {
        void UpdateValue(T newValue);
    }

    /// <summary>
    /// This class is used to observe the change of the in game data and notify listener for those changes
    /// </summary>
    public sealed class DataBindingRegistry : SingletonMono<DataBindingRegistry>
    {
        /// <summary>
        /// Backing field of <see cref="BindingProvider"/>
        /// </summary>
        private IDictionary<System.Type, object> m_bindingProvider;

        private IDictionary<System.Type, object> BindingProvider => m_bindingProvider ??= new Dictionary<Type, object>();

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            foreach (var provider in m_bindingProvider.Values)
            {
                if (provider is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            m_bindingProvider.Clear();
        }

        public DataBindingReactive<T> GetOrCreateProvider<T>()
        {
            Type key = typeof(T);

            if (!BindingProvider.TryGetValue(key, out object dataBinding))
            {
                dataBinding = new DataBindingReactive<T>();
                BindingProvider[key] = dataBinding;
            }

            if (dataBinding == null)
            {
                dataBinding = new DataBindingReactive<T>();
                BindingProvider[key] = dataBinding;
            }

            DataBindingReactive<T> final = dataBinding as DataBindingReactive<T>;
            
            if (final == null)
            {
                PDebug.ErrorFormat("[DataBindingRegistry] No DataBinding of type {0} match the return type {1}", typeof(T).Name,
                    typeof(ReactiveProperty<T>).Name);
                throw new NullReferenceException();
            }

            return final;
        }

        public IDisposable Bind<T>(Action<T> handler)
        {
            ReactiveProperty<T> provider = GetOrCreateProvider<T>();
            if (provider != null)
            {
                provider.Subscribe(handler);
            }

            return provider;
        }
    }
    
    /// <summary>
    /// Struct for change the ingame data therefore notify the subscriber for changes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct LazyInlet<T> : IDataInlet<T>
    {
        private DataBindingReactive<T> Provider { get; set; }
        
        public void UpdateValue(T newValue)
        {
            Provider ??= DataBindingRegistry.Instance.GetOrCreateProvider<T>();

            Provider.Value = newValue;
        }

        public void Dispose()
        {
            if (Provider == null)
            {
                return;
            }
            
            Provider.Dispose();
            Provider = null;
        }
    }
}