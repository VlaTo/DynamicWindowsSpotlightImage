using System;

namespace LibraProgramming.Windows.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    public class SubscriptionToken : IEquatable<SubscriptionToken>, IDisposable
    {
        private Action<SubscriptionToken> release;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="release"></param>
        public SubscriptionToken(Action<SubscriptionToken> release)
        {
            if (null == release)
            {
                throw new ArgumentNullException(nameof(release));
            }

            this.release = release;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as SubscriptionToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(SubscriptionToken other)
        {
            if (null == other)
            {
                return false;
            }

            return ReferenceEquals(this, other);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (null == release)
            {
                return;
            }

            release.Invoke(this);
            release = null;
        }
    }
}