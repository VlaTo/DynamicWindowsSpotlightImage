using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LibraProgramming.Windows.Async
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(AsyncWaitQueue<>.DebugView))]
    public sealed class AsyncWaitQueue<T> : IAsyncWaitQueue<T>
    {
        private readonly Deque<TaskCompletionSource<T>> queue;

        /// <summary>
        /// 
        /// </summary>
        public bool IsEmpty => 0 == Count;

        internal int Count
        {
            get
            {
                lock (queue)
                {
                    return queue.Count;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public AsyncWaitQueue()
        {
            queue = new Deque<TaskCompletionSource<T>>();
        }

        public Task<T> EnqueueAsync()
        {
            var tcs = new TaskCompletionSource<T>();

            lock (queue)
            {
                queue.Append(tcs);
            }

            return tcs.Task;
        }

        public IDisposable Dequeue(T result = default(T))
        {
            TaskCompletionSource<T> tcs;

            lock (queue)
            {
                tcs = queue.Dequeue();
            }

            return new CompleteDisposable(result, tcs);
        }

        public IDisposable DequeueAll(T result = default(T))
        {
            TaskCompletionSource<T>[] tcs;

            lock (queue)
            {
                tcs = queue.ToArray();
            }

            return new CompleteDisposable(result, tcs);
        }

        public IDisposable TryCancel(Task task)
        {
            TaskCompletionSource<T> tcs = null;

            lock (queue)
            {
                for (var index = 0; index < queue.Count; index++)
                {
                    var item = queue[index];

                    if (task == item.Task)
                    {
                        tcs = item;
                        queue.RemoveAt(index);

                        break;
                    }
                }
            }

            if (null == tcs)
            {
                return new CancelDisposable();
            }

            return new CancelDisposable(tcs);
        }

        public IDisposable CancelAll()
        {
            TaskCompletionSource<T>[] tcs;

            lock (queue)
            {
                tcs = queue.ToArray();
                queue.Clear();
            }

            return new CancelDisposable(tcs);
        }

        private sealed class CancelDisposable : IDisposable
        {
            private readonly TaskCompletionSource<T>[] sources;

            public CancelDisposable(params TaskCompletionSource<T>[] sources)
            {
                this.sources = sources;
            }

            void IDisposable.Dispose()
            {
                foreach (var tcs in sources)
                {
                    tcs.TrySetCanceled();
                }
            }
        }

        private sealed class CompleteDisposable : IDisposable
        {
            private readonly T result;
            private readonly TaskCompletionSource<T>[] sources;

            public CompleteDisposable(T result, params TaskCompletionSource<T>[] sources)
            {
                this.result = result;
                this.sources = sources;
            }

            void IDisposable.Dispose()
            {
                foreach (var tcs in sources)
                {
                    tcs.TrySetResult(result);
                }
            }
        }

        [DebuggerNonUserCode]
        internal sealed class DebugView
        {
            private readonly AsyncWaitQueue<T> queue;

            public DebugView(AsyncWaitQueue<T> queue)
            {
                this.queue = queue;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public Task<T>[] Tasks
            {
                get
                {
                    return queue.queue.Select(tcs => tcs.Task).ToArray();
                }
            } 
        }
    }
}