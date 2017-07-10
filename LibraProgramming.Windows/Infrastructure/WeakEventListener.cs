using System;
using Windows.Foundation;

namespace LibraProgramming.Windows.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    public static class WeakEventListener
	{
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TEventArgs"></typeparam>
        /// <param name="subscribe"></param>
        /// <param name="cleanup"></param>
        /// <param name="action"></param>
		public static IDisposable AttachEvent<TSource, TEventArgs>(
			Action<TypedEventHandler<TSource, TEventArgs>> subscribe,
			Action<TypedEventHandler<TSource, TEventArgs>> cleanup,
			TypedEventHandler<TSource, TEventArgs> action)
		{
			return new WeakEventListenerInternal<TSource, TEventArgs>(subscribe, cleanup, action);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="subscribe"></param>
        /// <param name="cleanup"></param>
        /// <param name="action"></param>
        public static IDisposable AttachEvent<TSource>(
            Action<Action<TSource>> subscribe,
            Action<Action<TSource>> cleanup,
            Action<TSource> action)
        {
            return new WeakEventListenerInternal<TSource>(subscribe, cleanup, action);
        }

		private class WeakEventListenerInternal<TSource, TEventArgs> : IDisposable
		{
			private readonly WeakDelegate<TypedEventHandler<TSource, TEventArgs>> action;
			private readonly Action<TypedEventHandler<TSource, TEventArgs>> cleanup;
		    private bool disposed;

			public WeakEventListenerInternal(
				Action<TypedEventHandler<TSource, TEventArgs>> subscribe,
				Action<TypedEventHandler<TSource, TEventArgs>> cleanup,
				TypedEventHandler<TSource, TEventArgs> action)
			{
				if (null == action)
				{
					throw new ArgumentNullException(nameof(action));
				}

			    this.action = new WeakDelegate<TypedEventHandler<TSource, TEventArgs>>(action);
				this.cleanup = cleanup;

				subscribe.Invoke(OnEvent);
			}

		    public void Dispose()
		    {
		        Dispose(true);
		    }

			private void OnEvent(TSource sender, TEventArgs e)
			{
			    if (action.IsAlive)
			    {
			        action.Invoke(sender, e);
			    }
			    else
			    {
			        Dispose(true);
			    }
			}

		    private void Dispose(bool dispose)
		    {
		        if (disposed)
		        {
		            return;
		        }

		        try
		        {
		            if (dispose)
		            {
		                cleanup.Invoke(OnEvent);
		            }
		        }
		        finally
		        {
		            disposed = true;
		        }
		    }
		}

        private class WeakEventListenerInternal<TSource> : IDisposable
        {
            private readonly WeakAction<TSource> action;
            private readonly Action<Action<TSource>> cleanup;
            private bool disposed;

            public WeakEventListenerInternal(
                Action<Action<TSource>> subscribe,
                Action<Action<TSource>> cleanup,
                Action<TSource> action)
            {
                if (null == action)
                {
                    throw new ArgumentNullException(nameof(action));
                }

                this.action = new WeakAction<TSource>(action);
                this.cleanup = cleanup;

                subscribe.Invoke(OnEvent);
            }

            public void Dispose()
            {
                Dispose(true);
            }

            private void OnEvent(TSource source)
            {
                if (action.IsAlive)
                {
                    action.Invoke(source);
                }
                else
                {
                    Dispose(true);
                }
            }

            private void Dispose(bool dispose)
            {
                if (disposed)
                {
                    return;
                }

                try
                {
                    if (dispose)
                    {
                        cleanup.Invoke(OnEvent);
                    }
                }
                finally
                {
                    disposed = true;
                }
            }
        }
    }
}