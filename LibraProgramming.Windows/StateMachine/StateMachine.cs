using System.Collections.Generic;

namespace LibraProgramming.Windows.StateMachine
{
    /// <summary>
    /// Simple implementation of the Finite State Machine pattern.
    /// </summary>
    /// <typeparam name="TState">The set of the states.</typeparam>
    /// <typeparam name="TTrigger">The set of the tiggers.</typeparam>
    public sealed class StateMachine<TState, TTrigger>
        where TState : struct
    {
        private readonly IDictionary<TState, StateDescriptor<TState, TTrigger>> states;
        private TState currentState;

        /// <summary>
        /// Gets the current state of the Finite State Machine.
        /// </summary>
        public TState CurrentState
        {
            get
            {
                return currentState;
            }
            private set
            {
                var comparer = EqualityComparer<TState>.Default;

                if (comparer.Equals(value, currentState))
                {
                    return;
                }

                DoCurrentStateExit();

                currentState = value;

                DoCurrentStateEnter();
            }
        }

        /// <summary>
        /// Creates empty Finite State Machine.
        /// </summary>
        public StateMachine()
            : this(default(TState))
        {
        }

        /// <summary>
        /// Creates new empty Finite State Machine with <paramref name="initialState"/> as a default state.
        /// </summary>
        /// <param name="initialState">The initial state.</param>
        public StateMachine(TState initialState)
        {
            states = new Dictionary<TState, StateDescriptor<TState, TTrigger>>(EqualityComparer<TState>.Default);
            currentState = initialState;
        }

        /// <summary>
        /// Configure the state.
        /// </summary>
        /// <param name="state">The particular <typeparamref name="TState"/> to configure.</param>
        /// <returns></returns>
        public IStateConfigurator<TState, TTrigger> Configure(TState state)
        {
            StateDescriptor<TState, TTrigger> descriptor;

            if (!states.TryGetValue(state, out descriptor))
            {
                descriptor = new StateDescriptor<TState, TTrigger>();
                states.Add(state, descriptor);
            }

            return new StateConfigurator<TState, TTrigger>(state, descriptor);
        }

        /// <summary>
        /// Gets value of trigger to fire transition.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        /// <returns>The value.</returns>
        /// <exception cref="StateMachineException"></exception>
        public bool CanFire(TTrigger trigger)
        {
            var descriptor = GetCurrentStateDescriptor();
            return descriptor.CanFire(trigger);
        }

        public void Fire(TTrigger trigger)
        {
            var descriptor = GetCurrentStateDescriptor();
            var transition = descriptor.GetTransition<PermittedStateTransition<TState>>(trigger);

            if (null == transition)
            {
                throw new StateMachineException();
            }

            if (!transition.CanTransit)
            {
                throw new StateMachineException();
            }

            CurrentState = transition.TargetState;
        }

        private void DoCurrentStateExit()
        {
            var descriptor = GetCurrentStateDescriptor();
            descriptor.OnExit?.Invoke(CurrentState);
        }

        private void DoCurrentStateEnter()
        {
            var descriptor = GetCurrentStateDescriptor();
            descriptor.OnEnter?.Invoke(CurrentState);
        }

        private StateDescriptor<TState, TTrigger> GetCurrentStateDescriptor()
        {
            StateDescriptor<TState, TTrigger> descriptor;

            if (!states.TryGetValue(CurrentState, out descriptor))
            {
                throw new StateMachineException();
            }

            return descriptor;
        }
    }
}