namespace LibraProgramming.Windows.StateMachine
{
    internal abstract class StateTransition
    {
        public abstract bool CanTransit
        {
            get;
        }
    }

    internal sealed class PermittedStateTransition<TState> : StateTransition
        where TState : struct
    {
        public TState TargetState
        {
            get;
        }

        public override bool CanTransit => true;

        public PermittedStateTransition(TState targetState)
        {
            TargetState = targetState;
        }
    }

    internal sealed class IgnoredTransition : StateTransition
    {
        public override bool CanTransit => false;
    }
}