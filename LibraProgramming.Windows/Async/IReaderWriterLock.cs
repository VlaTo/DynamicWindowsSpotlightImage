using System.Threading;

namespace LibraProgramming.Windows.Async
{
    /// <summary>
    /// 
    /// </summary>
    public interface IReaderWriterLock : IReaderLockProvider, IWriterLockProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        AwaitableDisposable<AsyncReaderWriterLock.UpgradeableReaderKey> AccquireUpgradeableReaderLockAsync(CancellationToken ct);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        AwaitableDisposable<AsyncReaderWriterLock.UpgradeableReaderKey> AccquireUpgradeableReaderLockAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        AsyncReaderWriterLock.UpgradeableReaderKey AccquireUpgradeableReaderLock(CancellationToken ct);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        AsyncReaderWriterLock.UpgradeableReaderKey AccquireUpgradeableReaderLock();
    }
}