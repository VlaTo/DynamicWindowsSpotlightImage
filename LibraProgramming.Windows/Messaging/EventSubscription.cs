using System.Threading.Tasks;

namespace LibraProgramming.Windows.Messaging
{
    internal class EventSubscription<TPayload> : IEventSubscription
    {
        protected readonly IActionReference<TPayload> Action;
        protected readonly IPredicateReference<TPayload> Filter;

        public SubscriptionToken Token
        {
            get;
            set;
        }

        public EventSubscription(IActionReference<TPayload> action, IPredicateReference<TPayload> filter)
        {
            Action = action;
            Filter = filter;
        }

        public virtual Task ExecuteAsync(params object[] args)
        {
            var payload = GetPalyload(args);

            ExecuteAction(payload);

            return Task.CompletedTask;
        }

        protected virtual void ExecuteAction(TPayload payload)
        {
            if (Filter.Invoke(payload))
            {
                Action.Invoke(payload);
            }
        }

        protected TPayload GetPalyload(object[] args)
        {
            var payload = default(TPayload);

            if (null != args && 0 < args.Length && null != args[0])
            {
                payload = (TPayload)args[0];
            }

            return payload;
        }
    }
}