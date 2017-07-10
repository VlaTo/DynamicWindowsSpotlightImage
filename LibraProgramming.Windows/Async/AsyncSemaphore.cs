using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using LibraProgramming.Windows.Infrastructure;

namespace LibraProgramming.Windows.Async
{
    /// <summary>
    /// An async-compatible semaphore. Alternatively, you could use <c>SemaphoreSlim</c> on .NET 4.5 / Windows Store.
    /// </summary>
    [DebuggerDisplay("Id = {Id}, CurrentCount = {count}")]
    [DebuggerTypeProxy(typeof(DebugView))]
    public sealed class AsyncSemaphore
    {
        private readonly IAsyncWaitQueue<object> queue;
        private int count;
        private int id;
        private readonly object mutex;

        /// <summary>
        /// 
        /// </summary>
        public int Id => IdManager<AsyncSemaphore>.GetId(ref id);

        /// <summary>
        /// 
        /// </summary>
        public int CurrentCount
        {
            get
            {
                lock (mutex)
                {
                    return count;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialCount"></param>
        /// <param name="queue"></param>
        public AsyncSemaphore(int initialCount, IAsyncWaitQueue<object> queue)
        {
            this.queue = queue;
            count = initialCount;
            mutex = new object();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialCount"></param>
        public AsyncSemaphore(int initialCount)
            : this(initialCount, new AsyncWaitQueue<object>())
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task WaitAsync(CancellationToken ct)
        {
            Task task;

            lock (mutex)
            {
                if (0 != count)
                {
                    count--;
                    task = Task.CompletedTask;
                }
                else
                {
                    task = queue.Enqueue(mutex, ct);
                }
            }

            return task;
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
            WaitAsync(ct).WaitAndUnwrapException();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Wait()
        {
            Wait(CancellationToken.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="releaseCount"></param>
        public void Release(int releaseCount)
        {
            if (0 == releaseCount)
            {
                return;
            }

            var filalizers = new List<IDisposable>();

            lock (mutex)
            {
                if (count > int.MaxValue - releaseCount)
                {
                    throw new InvalidOperationException("");
                }

                while (0 != releaseCount)
                {
                    if (queue.IsEmpty)
                    {
                        count++;
                    }
                    else
                    {
                        filalizers.Add(queue.Dequeue());
                    }

                    releaseCount--;
                }
            }

            foreach (var filalizer in filalizers)
            {
                filalizer.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Release()
        {
            Release(1);
        }

        // ReSharper disable UnusedMember.Local
        [DebuggerNonUserCode]
        private sealed class DebugView
        {
            private readonly AsyncSemaphore semaphore;

            public int Id => semaphore.Id;

            public int CurrentCount => semaphore.count;

            public IAsyncWaitQueue<object> WaitQueue => semaphore.queue;

            public DebugView(AsyncSemaphore semaphore)
            {
                this.semaphore = semaphore;
            }
        }
        //ReSharper restore UnusedMember.Local
    }
}