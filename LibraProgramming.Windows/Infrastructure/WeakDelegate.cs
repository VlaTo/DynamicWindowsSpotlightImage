using System;

namespace LibraProgramming.Windows.Infrastructure
{
    public sealed class WeakDelegate<TDelegate> : WeakDelegateBase, IEquatable<TDelegate>
    {
        public WeakDelegate(Delegate @delegate)
            : base(@delegate)
        {
        }

        public TDelegate CreateDelegate()
        {
            return (TDelegate) (object) CreateDelegate<TDelegate>();
        }

        public bool Equals(TDelegate other)
        {
            return Equals((Delegate) (object) other);
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other is WeakDelegate<TDelegate> && Equals((WeakDelegate<TDelegate>) other);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(WeakDelegate<TDelegate> left, WeakDelegate<TDelegate> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(WeakDelegate<TDelegate> left, WeakDelegate<TDelegate> right)
        {
            return false == Equals(left, right);
        }

        public void Invoke(params object[] args)
        {
            CreateDelegate<TDelegate>().DynamicInvoke(args);
        }

        private bool Equals(WeakDelegate<TDelegate> other)
        {
            if (null != Instance)
            {
                return Instance.Equals(other.Instance) && Method.Equals(other.Method);
            }

            if (null != other.Instance)
            {
                return false;
            }

            return Method.Equals(other.Method);
        }
    }
}
