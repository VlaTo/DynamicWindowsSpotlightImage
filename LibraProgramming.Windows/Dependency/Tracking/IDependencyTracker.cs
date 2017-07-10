using System;

namespace LibraProgramming.Windows.Dependency.Tracking
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IDependencyTracker<in TModel>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        IDependencyTracketSubscription Subscribe(TModel target);
    }
}