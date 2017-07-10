using System;
using System.Threading.Tasks;
using LibraProgramming.Windows.Infrastructure;

namespace LibraProgramming.Windows.Async
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class DeferralManager : IDeferrable
    {
        private AsyncCountdownEvent @event;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDisposable GetDeferral()
        {
            if (null == @event)
            {
                @event = new AsyncCountdownEvent(1);
            }

            @event.AddCount();

            return new DisposableToken<AsyncCountdownEvent>(@event, ReleaseDeferral);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task SignalAndWaitAsync()
        {
            if (null == @event)
            {
                return Task.CompletedTask;
            }

            @event.Signal();

            return @event.WaitAsync();
        }

        private static void ReleaseDeferral(AsyncCountdownEvent @event)
        {
            @event.Signal();
        }
    }
}