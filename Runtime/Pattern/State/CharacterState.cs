using Base.Utilities;
using UnityEngine;
using Base.Helper;

namespace Base.Pattern
{

    public interface IUpdateable
    {
        void PreUpdateBehaviour(float dt);
        void UpdateBehaviour(float dt);

        void FixedUpdateBehaviour(float dt);
        void PostUpdateBehaviour(float dt);
    }
    public abstract class CharacterState: BaseMono, IUpdateable
    {
        [SerializeField] private bool isOverrideAnimator = false;
        [SerializeField, Condition("isOverrideAnimator", true)] 
        private RuntimeAnimatorController runtimeAnimatorController = null;
        
        protected CharacterStateController CharacterStateController { get; private set; }

        public RuntimeAnimatorController RuntimeAnimator => runtimeAnimatorController;
        
        public int StateNameHash { get; private set; }

        public bool IsOverrideAnimator => isOverrideAnimator;

        protected virtual void Awake()
        {
            CharacterStateController = this.GetComponentInBranch<CharacterStateController>();
        }

        protected virtual void Start()
        {
            StateNameHash = Animator.StringToHash(this.GetType().Name);
        }
        
        protected virtual void OnEnable() {}
        
        protected virtual void OnDisable() {}
        
        protected virtual void Update() {}

        protected virtual void OnDestroy() {}

        /// <summary>
        /// This method runs once when the state has entered the state machine
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="fromState"></param>
        public virtual void EnterState(float dt, CharacterState fromState) { }
        
        /// <summary>
        /// This method runs once when the state has exited the state machine
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="toState"></param>
        public virtual void ExitStateBehaviour(float dt, CharacterState toState) {}

        
        /// <summary>
        /// This method runs before the main Update method
        /// </summary>
        /// <param name="dt"></param>
        public virtual void PreUpdateBehaviour(float dt) { }

        /// <summary>
        /// This method runs every frame, and should be implement by derived class
        /// </summary>
        /// <param name="dt"> The fixed delta time in every frame </param>
        public virtual void UpdateBehaviour(float dt) {}

        public abstract void FixedUpdateBehaviour(float dt);

        /// <summary>
        /// This method runs after the main Update method
        /// </summary>
        /// <param name="dt"></param>
        public virtual void PostUpdateBehaviour(float dt) { }
        
        /// <summary>
        /// Checks if the required conditions to exit this state are true. If so it returns the desired state (null otherwise). After this the state machine will
        /// proceed to evaluate the "enter transition" condition on the target state.
        /// </summary>
        public virtual void CheckExitTransition()
        {
        }

        /// <summary>
        /// Checks if the required conditions to enter this state are true. If so the state machine will automatically change the current state to the desired one.
        /// </summary>
        /// <param name="fromState"></param>
        public virtual bool CheckEnterTransition( CharacterState fromState )
        {
            return true;
        }
    }
}

