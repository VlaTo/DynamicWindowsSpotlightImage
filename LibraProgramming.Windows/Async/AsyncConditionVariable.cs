using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using LibraProgramming.Windows.Infrastructure;

namespace LibraProgramming.Windows.Async
{
    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("Id = {Id}, AsyncLockId = {@lock.Id}")]
    [DebuggerTypeProxy(typeof(DebugView))]
    public class AsyncConditionVariable
    {
        private int id;
        private readonly AsyncLock @lock;
        private readonly IAsyncWaitQueue<object> queue;
        private readonly object mutex;

        /// <summary>
        /// 
        /// </summary>
        public int Id => IdManager<AsyncConditionVariable>.GetId(ref id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lock"></param>
        /// <param name="queue"></param>
        public AsyncConditionVariable(AsyncLock @lock, IAsyncWaitQueue<object> queue)
        {
            mutex = new object();
            this.@lock = @lock;
            this.queue = queue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lock"></param>
        public AsyncConditionVariable(AsyncLock @lock)
            : this(@lock, new AsyncWaitQueue<object>())
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public void Notify()
        {
            IDisposable finish = null;

            lock (mutex)
            {
                if (false == queue.IsEmpty)
                {
                    finish = queue.Dequeue();
                }
            }

            finish?.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        public void NotifyAll()
        {
            IDisposable finish;

            lock (mutex)
            {
                finish = queue.DequeueAll();
            }

            finish?.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task WaitAsync(CancellationToken ct)
        {
            lock (mutex)
            {
                var tcs = new TaskCompletionSource();

                queue
                    .Enqueue(mutex, ct)
                    .ContinueWith(async temp =>
                    {
                        await @lock.LockAsync().ConfigureAwait(false);
                        tcs.TryCompleteFromTask(temp);
                    }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

                var task = tcs.Task;

                @lock.ReleaseLock();

                return task;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task WaitAsync()
        {
            return WaitAsync(CancellationToken.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ct"></param>
        public void Wait(CancellationToken ct)
        {
            Task task;

            lock (mutex)
            {
                task = queue.Enqueue(mutex, ct);
            }

            @lock.ReleaseLock();

            task.WaitWithoutException();

            @lock.Lock();

            task.WaitAndUnwrapException();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Wait()
        {
            Wait(CancellationToken.None);
        }

        // ReSharper disable UnusedMember.Local
        [DebuggerNonUserCode]
        private sealed class DebugView
        {
            private readonly AsyncConditionVariable acv;

            public int Id => acv.Id;

            public AsyncLock AsyncLock => acv.@lock;

            public IAsyncWaitQueue<object> WaitQueue => acv.queue;

            public DebugView(AsyncConditionVariable acv)
            {
                this.acv = acv;
            }
        }
        // ReSharper restore UnusedMember.Local
    }
}