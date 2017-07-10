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
    [DebuggerDisplay("Id = {Id}")]
    [DebuggerTypeProxy(typeof(DebugView))]
    public sealed class AsyncLock
    {
        private int id;
        private bool taken;
        private readonly object mutex;
        private readonly IAsyncWaitQueue<IDisposable> queue;
        private readonly Task<IDisposable> cachedKeyTask;

        /// <summary>
        /// 
        /// </summary>
        public int Id => IdManager<AsyncLock>.GetId(ref id);

        /// <summary>
        /// 
        /// </summary>
        public AsyncLock()
            : this(new AsyncWaitQueue<IDisposable>())
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queue"></param>
        public AsyncLock(IAsyncWaitQueue<IDisposable> queue)
        {
            cachedKeyTask = Task.FromResult<IDisposable>(new Key(this));
            this.queue = queue;
            mutex = new object();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public AwaitableDisposable<IDisposable> LockAsync(CancellationToken ct)
        {
            Task<IDisposable> task;

            lock (mutex)
            {
                if (false == taken)
                {
                    taken = true;
                    task = cachedKeyTask;
                }
                else
                {
                    task = queue.Enqueue(mutex, ct);
                }
            }

            return new AwaitableDisposable<IDisposable>(task);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public AwaitableDisposable<IDisposable> LockAsync()
        {
            return LockAsync(CancellationToken.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public IDisposable Lock(CancellationToken ct)
        {
            Task<IDisposable> task;

            lock (mutex)
            {
                if (false == taken)
                {
                    taken = true;
                    return cachedKeyTask.Result;
                }

                task = queue.Enqueue(mutex, ct);
            }

            return task.WaitAndUnwrapException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDisposable Lock()
        {
            return Lock(CancellationToken.None);
        }

        /// <summary>
        /// 
        /// </summary>
        internal void ReleaseLock()
        {
            IDisposable finish = null;

            lock (mutex)
            {
                if (queue.IsEmpty)
                {
                    taken = false;
                }
                else
                {
                    finish = queue.Dequeue(cachedKeyTask.Result);
                }
            }

            finish?.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class Key : IDisposable
        {
            private readonly AsyncLock @lock;

            public Key(AsyncLock @lock)
            {
                this.@lock = @lock;
            }

            public void Dispose()
            {
                @lock.ReleaseLock();
            }
        }

        //ReSharper disable UnusedMember.Local
        [DebuggerNonUserCode]
        private sealed class DebugView
        {
            private readonly AsyncLock mutex;

            public int Id => mutex.Id;

            public bool Taken => mutex.taken;

            public IAsyncWaitQueue<IDisposable> WaitQueue => mutex.queue;

            public DebugView(AsyncLock mutex)
            {
                this.mutex = mutex;
            }
        }
        //ReSharper restore UnusedMember.Local
    }
}