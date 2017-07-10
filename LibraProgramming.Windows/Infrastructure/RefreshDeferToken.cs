using System;

namespace LibraProgramming.Windows.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    public class RefreshDeferToken : IDisposable
    {
        private readonly IDeferrableRefresh instance;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        public RefreshDeferToken(IDeferrableRefresh instance)
        {
            if (null == instance)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            this.instance = instance;

            instance.DeferLevel++;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (1 > instance.DeferLevel)
            {
                return;
            }

            if (0 == --instance.DeferLevel)
            {
                instance.Refresh();
            }
        }
    }
}