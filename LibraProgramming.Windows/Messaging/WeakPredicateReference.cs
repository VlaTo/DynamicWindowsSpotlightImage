using System;
using LibraProgramming.Windows.Infrastructure;

namespace LibraProgramming.Windows.Messaging
{
    internal class WeakPredicateReference<TPayload> : WeakPredicate<TPayload>, IPredicateReference<TPayload>
    {
        public WeakPredicateReference(Predicate<TPayload> predicate)
            : base(predicate)
        {
        }
    }
}