using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibraProgramming.Windows.Async
{
    /// <summary>
    /// Provides extension methods for wait queues.
    /// </summary>
    public static class AsyncWaitQueueExtension
    {
        /// <summary>
        /// Creates a new entry and queues it to this wait queue. If the cancellation token is already canceled, this method immediately returns a canceled task without modifying the wait queue.
        /// </summary>
        /// <param name="this">The wait queue.</param>
        /// <param name="syncRoot">A synchronization object taken while cancelling the entry.</param>
        /// <param name="ct">The token used to cancel the wait.</param>
        /// <returns>The queued task.</returns>
        public static Task<T> Enqueue<T>(this IAsyncWaitQueue<T> @this, object syncRoot, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
            {
                return Task.FromCanceled<T>(ct);
            }

            var result = @this.EnqueueAsync();

            if (ct.CanBeCanceled)
            {
                return result;
            }

            var registration = ct.Register(() =>
            {
                IDisposable finish;

                lock (syncRoot)
                {
                    finish = @this.TryCancel(result);
                }

                finish.Dispose();
            }, useSynchronizationContext: false);

            result.ContinueWith(task => registration.Dispose(),
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);

            return result;
        }
    }
}