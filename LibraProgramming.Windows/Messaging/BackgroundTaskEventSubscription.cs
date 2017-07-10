using System.Threading.Tasks;

namespace LibraProgramming.Windows.Messaging
{
    internal class BackgroundTaskEventSubscription<TPayload> : EventSubscription<TPayload>
    {
        public BackgroundTaskEventSubscription(IActionReference<TPayload> action, IPredicateReference<TPayload> filter)
            : base(action, filter)
        {
        }

        public override async Task ExecuteAsync(params object[] args)
        {
            var payload = GetPalyload(args);
            await new Task(() => ExecuteAction(payload), TaskCreationOptions.DenyChildAttach).ConfigureAwait(false);
        }
    }
}