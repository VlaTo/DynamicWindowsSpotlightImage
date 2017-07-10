using System;

namespace LibraProgramming.Windows.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class WeakPredicate<TResult> : WeakDelegateBase, IEquatable<Predicate<TResult>>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        public WeakPredicate(Predicate<TResult> predicate)
            : base(predicate)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Predicate<TResult> CreateDelegate()
        {
            return (Predicate<TResult>) CreateDelegate<Predicate<TResult>>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Predicate<TResult> other)
        {
            return base.Equals(other);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Invoke(TResult value)
        {
            return (bool) CreateDelegate<Predicate<TResult>>().DynamicInvoke(value);
        }
    }
}
