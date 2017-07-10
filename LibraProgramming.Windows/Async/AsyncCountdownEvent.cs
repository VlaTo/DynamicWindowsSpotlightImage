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
    [DebuggerDisplay("Id = {Id}, Count = {count}")]
    [DebuggerTypeProxy(typeof(DebugView))]
    public sealed class AsyncCountdownEvent
    {
        private readonly TaskCompletionSource tcs;
        private int count;

        /// <summary>
        /// 
        /// </summary>
        public int Id => tcs.Task.Id;

        /// <summary>
        /// 
        /// </summary>
        public int Count => Interlocked.CompareExchange(ref count, 0, 0);

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:System.Object"/>.
        /// </summary>
        public AsyncCountdownEvent(int count)
        {
            tcs = new TaskCompletionSource();
            this.count = count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task WaitAsync()
        {
            return tcs.Task;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Wait()
        {
            WaitAsync().Wait();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ct"></param>
        public void Wait(CancellationToken ct)
        {
            var task = WaitAsync();

            if (task.IsCompleted)
            {
                return;
            }

            task.Wait(ct);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signalCount"></param>
        /// <returns></returns>
        public bool TryAddCount(int signalCount = 1)
        {
            return ModifyCount(signalCount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signalCount"></param>
        /// <returns></returns>
        public bool TrySignal(int signalCount = 1)
        {
            return ModifyCount(-signalCount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signalCount"></param>
        public void AddCount(int signalCount = 1)
        {
            if (false == ModifyCount(signalCount))
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signalCount"></param>
        public void Signal(int signalCount = 1)
        {
            if (false == ModifyCount(-signalCount))
            {
                throw new InvalidOperationException();
            }
        }

        private bool ModifyCount(int signalCount)
        {
            while (true)
            {
                var current = Count;

                if (0 == current)
                {
                    return false;
                }

                var value = current + signalCount;

                if (0 > value)
                {
                    return false;
                }

                if (current == Interlocked.CompareExchange(ref count, value, current))
                {
                    if (0 == value)
                    {
                        tcs.Complete();
                    }

                    return true;

                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DebuggerNonUserCode]
        private sealed class DebugView
        {
            private readonly AsyncCountdownEvent @event;

            public int Id => @event.Id;

            public int Count => @event.Count;

            public Task Task => @event.tcs.Task;

            /// <summary>
            /// Инициализирует новый экземпляр класса <see cref="T:System.Object"/>.
            /// </summary>
            public DebugView(AsyncCountdownEvent @event)
            {
                this.@event = @event;
            }
        }
    }
}