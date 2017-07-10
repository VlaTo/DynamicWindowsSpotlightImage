using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LibraProgramming.Windows.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    public class EventBase
    {
        private readonly IList<IEventSubscription> subscriptions;

        /// <summary>
        /// 
        /// </summary>
        public SynchronizationContext SynchronizationContext
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyCollection<IEventSubscription> Subscriptions => new ReadOnlyCollection<IEventSubscription>(subscriptions);

        internal EventBase()
        {
            subscriptions = new List<IEventSubscription>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual bool Contains(SubscriptionToken token)
        {
            if (null == token)
            {
                throw new ArgumentNullException(nameof(token));
            }

            lock (((ICollection)subscriptions).SyncRoot)
            {
                return subscriptions.Any(subscription => subscription.Token.Equals(token));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        public virtual void Unsubscribe(SubscriptionToken token)
        {
            if (null == token)
            {
                throw new ArgumentNullException(nameof(token));
            }

            lock (((ICollection)subscriptions).SyncRoot)
            {
                var subscription = subscriptions.FirstOrDefault(s => s.Token.Equals(token));

                if (null != subscription)
                {
                    subscriptions.Remove(subscription);
                }
            }
        }

        protected virtual SubscriptionToken SubscribeInternal(IEventSubscription subscription)
        {
            subscription.Token = new SubscriptionToken(Unsubscribe);

            lock (((ICollection)subscriptions).SyncRoot)
            {
                subscriptions.Add(subscription);
            }

            return subscription.Token;
        }

        protected virtual Task SendInternalAsync(params object[] args)
        {
            var tasks = GetActiveSubscriptions()
                .Select(subscription => subscription.ExecuteAsync(args));
            return Task.WhenAll(tasks);
        }

        private IEnumerable<IEventSubscription> GetActiveSubscriptions()
        {
            var result = new List<IEventSubscription>();

            lock (((ICollection)subscriptions).SyncRoot)
            {
                for (var index = subscriptions.Count - 1; index >= 0; index--)
                {
                    var strategy = subscriptions[index];

                    if (null == strategy)
                    {
                        subscriptions.RemoveAt(index);
                    }
                    else
                    {
                        result.Add(strategy);
                    }
                }
            }

            return result;
        }
    }
}