using System;

namespace LibraProgramming.Windows.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDeferrable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IDisposable GetDeferral();
    }
}