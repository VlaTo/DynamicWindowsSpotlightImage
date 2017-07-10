using System;

namespace LibraProgramming.Windows.Dependency
{
    /// <summary>
    /// 
    /// </summary>
    public interface IObservablePropertyTrackerSubscription : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IDisposable Disable();
    }
}