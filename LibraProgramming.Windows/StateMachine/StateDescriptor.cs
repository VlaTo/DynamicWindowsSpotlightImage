using System;
using System.Collections.Generic;

namespace LibraProgramming.Windows.StateMachine
{
    internal class StateDescriptor<TState, TTrigger>
    {
        public IDictionary<TTrigger, StateTransition> Triggers
        {
            get;
        }

        public Action<TState> OnExit
        {
            get;
            set;
        }

        public Action<TState> OnEnter
        {
            get;
            set;
        }

        public StateDescriptor()
        {
            Triggers = new Dictionary<TTrigger, StateTransition>(EqualityComparer<TTrigger>.Default);
        }

        public bool CanFire(TTrigger trigger)
        {
            StateTransition transition;

            if (!Triggers.TryGetValue(trigger, out transition))
            {
                throw new ArgumentException(nameof(trigger));
            }

            return transition.CanTransit;
        }

        public TTransition GetTransition<TTransition>(TTrigger trigger)
            where TTransition : StateTransition
        {
            StateTransition transition;
            return Triggers.TryGetValue(trigger, out transition) ? transition as TTransition : null;
        }
    }
}