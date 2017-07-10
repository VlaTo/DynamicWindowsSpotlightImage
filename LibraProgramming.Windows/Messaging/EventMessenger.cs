using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace LibraProgramming.Windows.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    public class EventMessenger : IEventMessenger
    {
        private readonly IDictionary<Type, EventBase> events;

        /// <summary>
        /// 
        /// </summary>
        public SynchronizationContext Context = SynchronizationContext.Current;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:System.Object"/>.
        /// </summary>
        public EventMessenger()
        {
            events = new Dictionary<Type, EventBase>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        public TEvent GetEvent<TEvent>()
            where TEvent : EventBase, new()
        {
            lock (((ICollection) events).SyncRoot)
            {
                var key = typeof (TEvent);
                EventBase @event;

                if (false == events.TryGetValue(key, out @event))
                {
                    @event = new TEvent
                    {
                        SynchronizationContext = Context
                    };

                    events[key] = @event;
                }

                return (TEvent) @event;
            }
        }
    }
}