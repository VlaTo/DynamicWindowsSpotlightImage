using System.Threading;
using System.Threading.Tasks;

namespace LibraProgramming.Windows.Messaging
{
    internal class DispatcherEventSubscription<TPayload> : EventSubscription<TPayload>
    {
        private readonly SynchronizationContext context;

        public DispatcherEventSubscription(IActionReference<TPayload> action, IPredicateReference<TPayload> filter, SynchronizationContext context)
            : base(action, filter)
        {
            this.context = context;
        }

        public override Task ExecuteAsync(params object[] args)
        {
            context.Post(payload => ExecuteAction((TPayload) payload), GetPalyload(args));
            return Task.CompletedTask;
        }
    }
}