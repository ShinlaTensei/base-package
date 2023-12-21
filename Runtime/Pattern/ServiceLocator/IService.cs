using System;
using System.Threading;
using Base.Helper;

namespace Base.Pattern
{
    public interface IService : IDisposable
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
        
        public virtual void Dispose() {}
    }
    
    public abstract class Service<T> : IService<T>
    {
        public bool IsInitialize { get; protected set; } = false;
        public virtual void UpdateData(T data) {}

        public virtual void Init()
        {
            IsInitialize = true;
        }
        
        public virtual void Dispose() {}
    }

    public abstract class MonoService : BaseMono, IService
    {
        public bool IsInitialize { get; protected set; } = false;

        public virtual void Init()
        {
            IsInitialize = true;
        }
        
        public virtual void Dispose() {}
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

