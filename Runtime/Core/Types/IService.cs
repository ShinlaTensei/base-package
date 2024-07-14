using System;
using System.Collections.Generic;
using System.Threading;
using Base.Helper;

namespace Base.Core
{
    public interface IService : IDisposable, IAuthHeaderProvider
    {
        bool IsInitialize { get; }
        void Init();
    }

    public abstract class Service : IService
    {
        public bool IsInitialize { get; protected set; } = false;

        public virtual void Init()
        {
            IsInitialize = true;
        }

        public virtual void Dispose()
        {
            IsInitialize = false;
        }

        public virtual Dictionary<string, string> GetAuthHeaders()
        {
            return new Dictionary<string, string>
            {
                {
                    "Authorization", "Bearer "
                }
            };
        }
    }
    
    public abstract class Service<T> : IService<T>
    {
        public bool IsInitialize { get; protected set; } = false;
        public virtual void UpdateData(T data) {}

        public virtual void Init()
        {
            IsInitialize = true;
        }

        public virtual void Dispose()
        {
            IsInitialize = false;
        }

        public virtual Dictionary<string, string> GetAuthHeaders()
        {
            return new Dictionary<string, string>
            {
                {
                    "Authorization", "Bearer "
                }
            };
        }
    }

    public interface IService<T> : IService
    {
        void UpdateData(T data);
    }

    public interface IServiceUpdate : IDisposable
    {
        void Update();
    }
}

