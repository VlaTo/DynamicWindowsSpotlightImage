using System;

namespace LibraProgramming.Windows.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    public class StrongPredicateReference<TPayload> : IPredicateReference<TPayload>
    {
        private readonly Predicate<TPayload> predicate;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        public StrongPredicateReference(Predicate<TPayload> predicate)
        {
            this.predicate = predicate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public bool Invoke(TPayload arg)
        {
            return predicate.Invoke(arg);
        }
    }
}