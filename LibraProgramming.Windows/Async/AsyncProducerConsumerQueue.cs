using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LibraProgramming.Windows.Infrastructure;

namespace LibraProgramming.Windows.Async
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class AsyncProducerConsumerQueue<T> : IDisposable
    {
        internal static readonly ReceiveResult FalseResult;

        private readonly Queue<T> queue;
        private readonly int maxCount;
        private readonly AsyncLock mutex;
        private readonly AsyncConditionVariable notFull;
        private readonly AsyncConditionVariable completedOrNotEmpty;
        private readonly CancellationTokenSource completed;

        /// <summary>
        /// 
        /// </summary>
        internal bool IsEmpty => 0 == queue.Count;

        /// <summary>
        /// 
        /// </summary>
        internal bool IsFull => maxCount == queue.Count;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="maxCount"></param>
        public AsyncProducerConsumerQueue(IEnumerable<T> collection, int maxCount)
        {
            if (0 >= maxCount)
            {
                throw new ArgumentOutOfRangeException(nameof(maxCount));
            }

            queue = null == collection ? new Queue<T>() : new Queue<T>(collection);

            if (queue.Count > maxCount)
            {
                throw new ArgumentException();
            }

            this.maxCount = maxCount;
            mutex = new AsyncLock();
            notFull = new AsyncConditionVariable(mutex);
            completedOrNotEmpty = new AsyncConditionVariable(mutex);
            completed = new CancellationTokenSource();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        public AsyncProducerConsumerQueue(IEnumerable<T> collection)
            : this(collection, int.MaxValue)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxCount"></param>
        public AsyncProducerConsumerQueue(int maxCount)
            : this(null, maxCount)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public AsyncProducerConsumerQueue()
            : this(null, int.MaxValue)
        {
        } 

        static AsyncProducerConsumerQueue()
        {
            FalseResult = new ReceiveResult(null, default(T));
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            completed.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Complete()
        {
            using (mutex.Lock())
            {
                if (completed.IsCancellationRequested)
                {
                    return;
                }

                completed.Cancel();
                completedOrNotEmpty.NotifyAll();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<bool> TryPostAsync(T item, CancellationToken ct)
        {
            var result = await TryPostAsync(item, ct, null).ConfigureAwait(false);

            if (null != result)
            {
                return true;
            }

            ct.ThrowIfCancellationRequested();

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Task<bool> TryPostAsync(T item)
        {
            return TryPostAsync(item, CancellationToken.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public bool TryPost(T item, CancellationToken ct)
        {
            var result = TryEnqueueInternal(item, ct);

            if (null != result)
            {
                return true;
            }

            ct.ThrowIfCancellationRequested();

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryPost(T item)
        {
            return TryPost(item, CancellationToken.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task PostAsync(T item, CancellationToken ct)
        {
            var result = await TryPostAsync(item, ct).ConfigureAwait(false);

            if (false == result)
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Task PostAsync(T item)
        {
            return PostAsync(item, CancellationToken.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="ct"></param>
        public void Post(T item, CancellationToken ct)
        {
            var result = TryPost(item, ct);

            if (false == result)
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Post(T item)
        {
            Post(item, CancellationToken.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<bool> OutputAvailableAsync(CancellationToken ct)
        {
            using (await mutex.LockAsync().ConfigureAwait(false))
            {
                while (false == completed.IsCancellationRequested && IsEmpty)
                {
                    await completedOrNotEmpty.WaitAsync(ct).ConfigureAwait(false);
                }

                return false == IsEmpty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<bool> OutputAvailableAsync()
        {
            return OutputAvailableAsync(CancellationToken.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public IEnumerable<T> ReceiveAll(CancellationToken ct)
        {
            while (true)
            {
                T item;

                if (false == TryReceiveInternal(out item, ct))
                {
                    yield break;
                }

                yield return item;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> ReceiveAll()
        {
            return ReceiveAll(CancellationToken.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<ReceiveResult> TryReceiveAsync(CancellationToken ct)
        {
            var result = await TryReceiveAsync(ct, null).ConfigureAwait(false);

            if (false == result.Success)
            {
                ct.ThrowIfCancellationRequested();
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<ReceiveResult> TryReceiveAsync()
        {
            return TryReceiveAsync(CancellationToken.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public bool TryReceive(out T item, CancellationToken ct)
        {
            if (TryReceiveInternal(out item, ct))
            {
                return true;
            }

            ct.ThrowIfCancellationRequested();

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool TryReceive(out T item)
        {
            return TryReceive(out item, CancellationToken.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<T> ReceiveAsync(CancellationToken ct)
        {
            var result = await TryReceiveAsync(ct).ConfigureAwait(false);

            if (false == result.Success)
            {
                ct.ThrowIfCancellationRequested();
            }

            return result.Item;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<T> ReceiveAsync()
        {
            return ReceiveAsync(CancellationToken.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public T Receive(CancellationToken ct)
        {
            T item;

            if (false == TryReceive(out item, ct))
            {
                throw new InvalidOperationException();
            }

            return item;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T Receive()
        {
            return Receive(CancellationToken.None);
        }

        internal async Task<AsyncProducerConsumerQueue<T>> TryPostAsync(T item, CancellationToken ct, TaskCompletionSource abort)
        {
            try
            {
                using (var source = CancellationTokenHelper.Aggregate(completed.Token, ct))
                {
                    using (await mutex.LockAsync().ConfigureAwait(false))
                    {
                        while (IsFull)
                        {
                            await notFull.WaitAsync(source.Token).ConfigureAwait(false);
                        }

                        if (completed.IsCancellationRequested)
                        {
                            return null;
                        }

                        if (null != abort && false == abort.TryCancel())
                        {
                            return null;
                        }

                        queue.Enqueue(item);
                        completedOrNotEmpty.Notify();

                        return this;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }

        internal async Task<ReceiveResult> TryReceiveAsync(CancellationToken ct, TaskCompletionSource abort)
        {
            try
            {
                using (await mutex.LockAsync().ConfigureAwait(false))
                {
                    while (false == completed.IsCancellationRequested && IsEmpty)
                    {
                        await completedOrNotEmpty.WaitAsync(ct).ConfigureAwait(false);
                    }

                    if (completed.IsCancellationRequested && IsEmpty)
                    {
                        return FalseResult;
                    }

                    if (null != abort && abort.TryCancel())
                    {
                        return FalseResult;
                    }

                    var item = queue.Dequeue();

                    notFull.Notify();

                    return new ReceiveResult(this, item);
                }
            }
            catch (OperationCanceledException)
            {
                return FalseResult;
            }
        }

        internal bool TryReceiveInternal(out T item, CancellationToken ct)
        {
            item = default(T);

            try
            {
                using (mutex.Lock())
                {
                    while (false == completed.IsCancellationRequested && IsEmpty)
                    {
                        completedOrNotEmpty.Wait(ct);
                    }

                    if (completed.IsCancellationRequested && IsEmpty)
                    {
                        return false;
                    }

                    item = queue.Dequeue();

                    notFull.Notify();

                    return true;
                }
            }
            catch (OperationCanceledException)
            {
                return false;
            }
        }

        internal AsyncProducerConsumerQueue<T> TryEnqueueInternal(T item, CancellationToken ct)
        {
            try
            {
                using (var source = CancellationTokenHelper.Aggregate(completed.Token, ct))
                {
                    using (mutex.Lock())
                    {
                        while (IsFull)
                        {
                            notFull.Wait(source.Token);
                        }

                        if (completed.IsCancellationRequested)
                        {
                            return null;
                        }

                        queue.Enqueue(item);
                        completedOrNotEmpty.Notify();

                        return this;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed class ReceiveResult
        {
            /// <summary>
            /// 
            /// </summary>
            private AsyncProducerConsumerQueue<T> Queue
            {
                get;
            }

            /// <summary>
            /// 
            /// </summary>
            public T Item
            {
                get;
            }

            /// <summary>
            /// 
            /// </summary>
            public bool Success => null != Queue;

            internal ReceiveResult(AsyncProducerConsumerQueue<T> queue, T item)
            {
                Queue = queue;
                Item = item;
            }
        }
    }
}