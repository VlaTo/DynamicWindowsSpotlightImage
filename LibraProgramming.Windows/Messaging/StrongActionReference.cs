using System;

namespace LibraProgramming.Windows.Messaging
{
    internal class StrongActionReference<TPayload> : IActionReference<TPayload>
    {
        private readonly Action<TPayload> action;

        public StrongActionReference(Action<TPayload> action)
        {
            this.action = action;
        }

        public void Invoke(TPayload arg)
        {
            action.Invoke(arg);
        }
    }
}