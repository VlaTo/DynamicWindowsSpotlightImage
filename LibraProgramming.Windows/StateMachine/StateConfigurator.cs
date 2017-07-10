using System;

namespace LibraProgramming.Windows.StateMachine
{
    internal class StateConfigurator<TState, TTrigger> : IStateConfigurator<TState, TTrigger>
        where TState : struct
    {
        private readonly StateDescriptor<TState, TTrigger> descriptor;
        private TState state;

        public StateConfigurator(TState state, StateDescriptor<TState, TTrigger> descriptor)
        {
            this.state = state;
            this.descriptor = descriptor;
        }

        public IStateConfigurator<TState, TTrigger> Permit(TTrigger trigger, TState target)
        {
            EnsureNoTrigger(trigger);

            descriptor.Triggers.Add(trigger, new PermittedStateTransition<TState>(target));

            return this;
        }

        public IStateConfigurator<TState, TTrigger> Ignore(TTrigger trigger)
        {
            EnsureNoTrigger(trigger);

            descriptor.Triggers.Add(trigger, new IgnoredTransition());

            return this;
        }

        public IStateConfigurator<TState, TTrigger> OnEnter(Action<TState> action)
        {
            if (null != descriptor.OnEnter)
            {
                throw new ArgumentException(nameof(action));
            }

            descriptor.OnEnter = action;

            return this;
        }

        public IStateConfigurator<TState, TTrigger> OnExit(Action<TState> action)
        {
            if (null != descriptor.OnExit)
            {
                throw new ArgumentException(nameof(action));
            }

            descriptor.OnExit = action;

            return this;
        }

        private void EnsureNoTrigger(TTrigger trigger)
        {
            if (descriptor.Triggers.ContainsKey(trigger))
            {
                throw new ArgumentException(nameof(trigger));
            }
        }
    }
}