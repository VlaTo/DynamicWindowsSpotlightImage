using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LibraProgramming.Windows.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    public static class CancellationTokenHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public static AggregatedCancellationToken Aggregate(params CancellationToken[] tokens)
        {
            return Aggregate((IEnumerable<CancellationToken>) tokens);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public static AggregatedCancellationToken Aggregate(IEnumerable<CancellationToken> tokens)
        {
            var cts = tokens.Where(token => token.CanBeCanceled).ToArray();

            switch (cts.Length)
            {
                case 0:
                {
                    return new AggregatedCancellationToken();
                }

                case 1:
                {
                    return new AggregatedCancellationToken(cts[0]);
                }

                default:
                {
                    var canceled = cts.FirstOrDefault(token => token.IsCancellationRequested);

                    if (canceled.IsCancellationRequested)
                    {
                        return new AggregatedCancellationToken(canceled);
                    }

                    return new AggregatedCancellationToken(CancellationTokenSource.CreateLinkedTokenSource(cts));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="continuationOptions"></param>
        /// <returns></returns>
        public static AggregatedCancellationToken FromTask(Task source, TaskContinuationOptions continuationOptions)
        {
            var cts = new CancellationTokenSource();

            source.ContinueWith(notused => cts.Cancel(), CancellationToken.None, continuationOptions, TaskScheduler.Default);

            return new AggregatedCancellationToken(cts);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static AggregatedCancellationToken FromTask(Task source)
        {
            return FromTask(source, TaskContinuationOptions.None);
        }
    }
}