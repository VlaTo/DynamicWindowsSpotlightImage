using System;
using LibraProgramming.Windows.Infrastructure;

namespace LibraProgramming.Windows.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    internal class WeakActionReference<TPayload> : WeakAction<TPayload>, IActionReference<TPayload>
    {
        public WeakActionReference(Action<TPayload> action)
            : base(action)
        {
        }
    }
}