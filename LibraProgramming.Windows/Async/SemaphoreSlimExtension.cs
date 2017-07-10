using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibraProgramming.Windows.Async
{
    /// <summary>
    /// Provides extension methods for <see cref="SemaphoreSlim" />.
    /// </summary>
    public static class SemaphoreSlimExtension
    {
        /// <summary>
        /// Asynchronously waits on the semaphore, and returns a disposable that releases the semaphore when disposed,
        /// thus treating this semaphore as a "multi-lock".
        /// </summary>
        /// <param name="semaphore">The semaphore to lock.</param>
        /// <param name="ctx">The cancellation token used to cancel the wait.</param>
        public static AwaitableDisposable<IDisposable> LockAsync(this SemaphoreSlim semaphore, CancellationToken ctx)
        {
            return new AwaitableDisposable<IDisposable>(DoLockAsync(semaphore, ctx));
        }

        /// <summary>
        /// Asynchronously waits on the semaphore, and returns a disposable that releases the semaphore when disposed,
        /// thus treating this semaphore as a "multi-lock".
        /// </summary>
        public static AwaitableDisposable<IDisposable> LockAsync(this SemaphoreSlim semaphore)
            => semaphore.LockAsync(CancellationToken.None);

        private static async Task<IDisposable> DoLockAsync(SemaphoreSlim semaphore, CancellationToken ctx)
        {
            await semaphore.WaitAsync(ctx).ConfigureAwait(false);
            return new DisposableToken(() => semaphore.Release());
        }
    }
}