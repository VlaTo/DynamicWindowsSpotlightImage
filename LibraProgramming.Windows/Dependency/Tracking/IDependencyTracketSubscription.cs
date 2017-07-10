using System;

namespace LibraProgramming.Windows.Dependency.Tracking
{
    public interface IDependencyTracketSubscription : IDisposable
    {
        IDisposable DisableTracking(bool forceSet);
    }
}