using System;
using Base.Helper;
using System.Collections.Generic;
using UnityEngine;

namespace Base.Pattern
{
    [AddComponentMenu("Base Component/Character State Controller")]
    public class CharacterStateController : BaseMono
    {
        [SerializeField] private Animator animator = null;
        
        [SerializeField] private CharacterState currentState = null;
        
        private Dictionary<string, CharacterState> _states = new Dictionary<string, CharacterState>();
        
        private Queue<CharacterState> _transitionQueue = new Queue<CharacterState>();

        private CharacterState _previousState = null;

        /// <summary>
        /// Event fire when a state transition occurs.
        /// </summary>
        public event Action<CharacterState, CharacterState> OnStateChanged;
        
        /// <summary>
        /// Gets the current state used by the state machine
        /// </summary>
        public CharacterState CurrentState => currentState;
        
        /// <summary>
        /// The animator associated with the state controller
        /// </summary>
        public Animator Animator { get => animator;
            set => animator = value;
        }
        
        /// <summary>
        /// Gets the previous state used by the state machine
        /// </summary>
        public CharacterState PreviousState => _previousState;

        private bool CanCurrentStateOverrideAnimator => CurrentState.IsOverrideAnimator && Animator != null && CurrentState.RuntimeAnimator != null;

        
        /// <summary>
        /// Get a particular state
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns></returns>
        public CharacterState GetState(string stateName)
        {
            CharacterState result = null;
            bool isValid = _states.TryGetValue(stateName, out result);

            return result;
        }

        public CharacterState GetState<T>() where T : CharacterState
        {
            foreach (var state in _states.Values)
            {
                if (state.GetType() == typeof(T))
                {
                    return state;
                }
            }

            return null;
        }
        
        /// <summary>
        /// Adds a particular state to the transition state queue (as a potential transition). The state machine will eventually check if the transition is accepted or rejected 
        /// by the target state (CheckEnterTransition). Call this method from within the CheckExitTransition method. 
        /// </summary>
        /// <example>
        /// For instance, if you need to transition to multiple states.
        /// <code>
        /// if( conditionA )
        /// {	
        /// 	EnqueueTransition<TargetStateA>();
        /// }
        /// else if( conditionB )
        /// {
        /// 	EnqueueTransition<TargetStateB>();
        /// 	EnqueueTransition<TargetStateC>(); 	
        /// }
        /// </code>	
        /// </example>
        public void EnqueueTransition<T>() where T : CharacterState
        {
            CharacterState state = GetState<T>();

            if( state != null )
                _transitionQueue.Enqueue( state );
        }
        
        // --------------------------- Unity Event function ------------------------------------------------ //
        private void Awake()
        {
            AddAndInitializeState();
        }

        protected override void Start()
        {
            base.Start();
            if (CurrentState != null)
            {
                CurrentState.EnterStateBehaviour(0, CurrentState);

                if (CanCurrentStateOverrideAnimator)
                {
                    Animator.runtimeAnimatorController = CurrentState.RuntimeAnimator;
                }
            }
        }

        private void FixedUpdate()
        {
            float dt = Time.deltaTime;

            if (CurrentState == null) return;

            bool changeOfState = CheckForTransition();
            
            // Reset the transition queue
            _transitionQueue.Clear();

            if (changeOfState)
            {
                PreviousState.ExitStateBehaviour(dt, CurrentState);
                
                if (CanCurrentStateOverrideAnimator)
                {
                    Animator.runtimeAnimatorController = CurrentState.RuntimeAnimator;
                }

                CurrentState.EnterStateBehaviour(dt, PreviousState);
            }
            
            CurrentState.PreUpdateBehaviour(dt);
            CurrentState.FixedUpdateBehaviour(dt);
            CurrentState.PostUpdateBehaviour(dt);
        }

        private void Update()
        {
            CurrentState.UpdateBehaviour(Time.deltaTime);
        }

        // --------------------------- End of Unity Event function ----------------------------------------- //
        
        private void AddAndInitializeState()
        {
            CharacterState[] stateArray = this.GetComponentsInChildren<CharacterState>();
            for (int i = 0; i < stateArray.Length; ++i)
            {
                CharacterState state = stateArray[i];
                string stateName = state.GetType().Name;
                if (GetState(stateName) != null)
                {
                    Debug.Log(String.Format("Warning: Game object {0} of {1} already has the state {2}",
                        new object[] {state.gameObject.name, state.transform.parent.gameObject.name, stateName}));
                    continue;
                }
                
                _states.Add(stateName, state);
            }
        }

        private bool CheckForTransition()
        {
            CurrentState.CheckExitTransition();

            CharacterState nextState = null;

            while (_transitionQueue.Count != 0)
            {
                CharacterState thisState = _transitionQueue.Dequeue();

                if (thisState == null) continue;

                if (!thisState.enabled)
                {
                    continue;
                }

                bool success = thisState.CheckEnterTransition(CurrentState);

                if (success)
                {
                    nextState = thisState;

                    OnStateChanged?.Invoke(CurrentState, nextState);

                    _previousState = CurrentState;
                    currentState = nextState;
                    currentState.CharacterStateController = this;

                    return true;
                }
            }

            return false;
        }
    }
}
