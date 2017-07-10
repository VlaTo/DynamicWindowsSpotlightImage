using System;
using System.Threading;

namespace LibraProgramming.Windows.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class AggregatedCancellationToken : IDisposable
    {
        private readonly CancellationTokenSource cts;

        public CancellationToken Token
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        public AggregatedCancellationToken()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cts"></param>
        public AggregatedCancellationToken(CancellationTokenSource cts)
        {
            this.cts = cts;
            Token = cts.Token;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ct"></param>
        public AggregatedCancellationToken(CancellationToken ct)
        {
            Token = ct;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            cts.Dispose();
        }
    }
}