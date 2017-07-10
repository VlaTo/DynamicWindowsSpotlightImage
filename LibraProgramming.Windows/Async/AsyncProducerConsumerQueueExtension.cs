using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibraProgramming.Windows.Infrastructure;

namespace LibraProgramming.Windows.Async
{
    /// <summary>
    /// 
    /// </summary>
    public static class AsyncProducerConsumerQueueExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queues"></param>
        /// <param name="item"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static async Task<AsyncProducerConsumerQueue<T>> TryEnqueueToAnyAsync<T>(
            this IEnumerable<AsyncProducerConsumerQueue<T>> queues, T item, CancellationToken ct)
        {
            var abort = new TaskCompletionSource();

            using (var cancellation = CancellationTokenHelper.FromTask(abort.Task))
            {
                using (var aggregation = CancellationTokenHelper.Aggregate(cancellation.Token, ct))
                {
                    var token = aggregation.Token;
                    var tasks = queues.Select(queue => queue.TryPostAsync(item, token, abort));
                    var results = await Task.WhenAll(tasks).ConfigureAwait(false);
                    var candidate = results.FirstOrDefault(value => null != value);

                    if (null == candidate)
                    {
                        ct.ThrowIfCancellationRequested();
                    }

                    return candidate;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queues"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static Task<AsyncProducerConsumerQueue<T>> TryEnqueueToAnyAsync<T>(
            this IEnumerable<AsyncProducerConsumerQueue<T>> queues, T item)
        {
            return TryEnqueueToAnyAsync(queues, item, CancellationToken.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queues"></param>
        /// <param name="item"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static AsyncProducerConsumerQueue<T> TryEnqueueToAny<T>(
            this IEnumerable<AsyncProducerConsumerQueue<T>> queues, T item, CancellationToken ct)
        {
            return TryEnqueueToAnyAsync(queues, item, ct).WaitAndUnwrapException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queues"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static AsyncProducerConsumerQueue<T> TryEnqueueToAny<T>(
            this IEnumerable<AsyncProducerConsumerQueue<T>> queues, T item)
        {
            return TryEnqueueToAny(queues, item, CancellationToken.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queues"></param>
        /// <param name="item"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static async Task<AsyncProducerConsumerQueue<T>> EqueueToAnyAsync<T>(
            this IEnumerable<AsyncProducerConsumerQueue<T>> queues, T item, CancellationToken ct)
        {
            var result = await TryEnqueueToAnyAsync(queues, item, ct).ConfigureAwait(false);

            if (null == result)
            {
                throw new InvalidOperationException();
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queues"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static Task<AsyncProducerConsumerQueue<T>> EqueueToAnyAsync<T>(
            this IEnumerable<AsyncProducerConsumerQueue<T>> queues, T item)
        {
            return EqueueToAnyAsync(queues, item, CancellationToken.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queues"></param>
        /// <param name="item"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static AsyncProducerConsumerQueue<T> EnqueueToAny<T>(IEnumerable<AsyncProducerConsumerQueue<T>> queues,
            T item, CancellationToken ct)
        {
            var result = TryEnqueueToAny(queues, item, ct);

            if (null == result)
            {
                throw new InvalidOperationException();
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queues"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static AsyncProducerConsumerQueue<T> EnqueueToAny<T>(IEnumerable<AsyncProducerConsumerQueue<T>> queues,
            T item)
        {
            return EnqueueToAny(queues, item, CancellationToken.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queues"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static async Task<AsyncProducerConsumerQueue<T>.ReceiveResult> TryDequeueFromAnyAsync<T>(
            this IEnumerable<AsyncProducerConsumerQueue<T>> queues, CancellationToken ct)
        {
            var abort = new TaskCompletionSource();

            using (var cancellation = CancellationTokenHelper.FromTask(abort.Task))
            {
                using (var aggregation = CancellationTokenHelper.Aggregate(cancellation.Token, ct))
                {
                    var token = aggregation.Token;
                    var tasks = queues.Select(queue => queue.TryReceiveAsync(token, abort));
                    var results = await Task.WhenAll(tasks).ConfigureAwait(false);
                    var result = results.FirstOrDefault(value => value.Success);

                    if (null != result)
                    {
                        return result;
                    }

                    ct.ThrowIfCancellationRequested();

                    return AsyncProducerConsumerQueue<T>.FalseResult;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queues"></param>
        /// <returns></returns>
        public static Task<AsyncProducerConsumerQueue<T>.ReceiveResult> TryDequeueFromAnyAsync<T>(
            this IEnumerable<AsyncProducerConsumerQueue<T>> queues)
        {
            return TryDequeueFromAnyAsync(queues, CancellationToken.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queues"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static AsyncProducerConsumerQueue<T>.ReceiveResult TryDequeueFromAny<T>(
            this IEnumerable<AsyncProducerConsumerQueue<T>> queues, CancellationToken ct)
        {
            return TryDequeueFromAnyAsync(queues, ct).WaitAndUnwrapException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queues"></param>
        /// <returns></returns>
        public static AsyncProducerConsumerQueue<T>.ReceiveResult TryDequeueFromAny<T>(
            this IEnumerable<AsyncProducerConsumerQueue<T>> queues)
        {
            return TryDequeueFromAny(queues, CancellationToken.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queues"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static async Task<AsyncProducerConsumerQueue<T>.ReceiveResult> DequeueFromAnyAsync<T>(
            this IEnumerable<AsyncProducerConsumerQueue<T>> queues, CancellationToken ct)
        {
            var result = await TryDequeueFromAnyAsync(queues, ct).ConfigureAwait(false);

            if (false == result.Success)
            {
                throw new InvalidOperationException();
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queues"></param>
        /// <returns></returns>
        public static Task<AsyncProducerConsumerQueue<T>.ReceiveResult> DequeueFromAnyAsync<T>(
            this IEnumerable<AsyncProducerConsumerQueue<T>> queues)
        {
            return DequeueFromAnyAsync(queues, CancellationToken.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queues"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static AsyncProducerConsumerQueue<T>.ReceiveResult DequeueFromAny<T>(
            this IEnumerable<AsyncProducerConsumerQueue<T>> queues, CancellationToken ct)
        {
            var result = TryDequeueFromAny(queues, ct);

            if (false == result.Success)
            {
                throw new InvalidOperationException();
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queues"></param>
        /// <returns></returns>
        public static AsyncProducerConsumerQueue<T>.ReceiveResult DequeueFromAny<T>(
            this IEnumerable<AsyncProducerConsumerQueue<T>> queues)
        {
            return DequeueFromAny(queues, CancellationToken.None);
        }
    }
}