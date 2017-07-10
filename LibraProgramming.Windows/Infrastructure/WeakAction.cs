using System;

namespace LibraProgramming.Windows.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    public class WeakAction : WeakDelegateBase, IEquatable<Action>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        public WeakAction(Action action)
            : base(action)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Action CreateDelegate()
        {
            return (Action) CreateDelegate<Action>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Action other)
        {
            return base.Equals(other);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Invoke()
        {
            CreateDelegate<Action>().DynamicInvoke();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class WeakAction<T> : WeakDelegateBase, IEquatable<Action<T>>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        public WeakAction(Action<T> action)
            : base(action)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Action<T> CreateDelegate()
        {
            return (Action<T>) CreateDelegate<Action>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Action<T> other)
        {
            return base.Equals(other);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        public void Invoke(T arg)
        {
            CreateDelegate<Action>().DynamicInvoke(arg);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class WeakAction<T1, T2> : WeakDelegateBase, IEquatable<Action<T1, T2>>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        public WeakAction(Action<T1, T2> action)
            : base(action)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Action<T1, T2> CreateDelegate()
        {
            return (Action<T1, T2>) CreateDelegate<Action>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Action<T1, T2> other)
        {
            return base.Equals(other);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        public void Invoke(T1 arg1, T2 arg2)
        {
            CreateDelegate<Action>().DynamicInvoke(arg1, arg2);
        }
    }
}