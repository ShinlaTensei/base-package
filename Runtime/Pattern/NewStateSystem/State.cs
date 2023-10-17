#region Header
// Date: 17/08/2023
// Created by: Huynh Phong Tran
// File name: State.cs
#endregion

namespace Base.Pattern
{
    public interface IState
    {
        bool IsReadyToExit();
        bool IsReadyToEnter();
        void OnEnter();
        void OnExit();
    }

    public abstract class State : IUpdateable, IState
    {
        public virtual void PreUpdateBehaviour(float dt)
        {
            
        }

        public virtual void UpdateBehaviour(float dt)
        {
            
        }

        public virtual void FixedUpdateBehaviour(float dt)
        {
            
        }

        public virtual void PostUpdateBehaviour(float dt)
        {
            
        }

        public abstract bool IsReadyToExit();

        public abstract bool IsReadyToEnter();

        public virtual void OnEnter()
        {
            
        }

        public virtual void OnExit()
        {
            
        }
    }
}
