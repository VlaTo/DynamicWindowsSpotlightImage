using System;

namespace LibraProgramming.Windows.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public sealed class WeakFunc<TResult> : WeakDelegateBase, IEquatable<Func<TResult>>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="func"></param>
        public WeakFunc(Func<TResult> func)
            : base(func)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Func<TResult> CreateDelegate()
        {
            return (Func<TResult>) CreateDelegate<Func<TResult>>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Func<TResult> other)
        {
            return base.Equals(other);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TResult Invoke()
        {
            return (TResult) CreateDelegate<Func<TResult>>().DynamicInvoke();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <typeparam name="T1"></typeparam>
    public sealed class WeakFunc<T1, TResult> : WeakDelegateBase, IEquatable<Func<T1, TResult>>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="func"></param>
        public WeakFunc(Func<T1, TResult> func)
            : base(func)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Func<T1, TResult> CreateDelegate()
        {
            return (Func<T1, TResult>) CreateDelegate<Func<T1, TResult>>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Func<T1, TResult> other)
        {
            return base.Equals(other);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public TResult Invoke(T1 value)
        {
            return (TResult) CreateDelegate<Func<T1, TResult>>().DynamicInvoke(value);
        }
    }
}
