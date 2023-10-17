using System;
using System.Threading;

namespace Base.Pattern
{
    public interface IService : IDisposable
    {
        void Init();
    }

    public abstract class Service : IService
    {
        public virtual void Init() { }
        
        public virtual void Dispose() {}
    }
    
    public abstract class Service<T> : IService<T>
    {
        public virtual void UpdateData(T data) {}
        public virtual void Init()             {}
        
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

