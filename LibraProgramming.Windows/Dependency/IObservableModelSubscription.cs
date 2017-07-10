using System;

namespace LibraProgramming.Windows.Dependency
{
    /// <summary>
    /// 
    /// </summary>
    public interface IObservableModelSubscription : IDisposable
    {
        IDisposable Disable();
    }
}