using System;

namespace LibraProgramming.Windows.Navigations
{
    /// <summary>
    /// 
    /// </summary>
    public static class PageNavigation
    {
        private static IPageNavigationProvider provider;

        public static IDisposable RegisterProvider(IPageNavigationProvider value)
        {
            if (null != provider)
            {
                throw new InvalidOperationException();
            }

            provider = value;

            return new SubscriptionToken(value);
        }

        public static bool NavigateToPage(Type targetPage, object parameter)
        {
            return provider.NavigateToPage(targetPage, parameter);
        }

        /// <summary>
        /// 
        /// </summary>
        private class SubscriptionToken : IDisposable
        {
            private readonly IPageNavigationProvider provider;

            public SubscriptionToken(IPageNavigationProvider provider)
            {
                this.provider = provider;
            }

            public void Dispose()
            {
                if (PageNavigation.provider != provider)
                {
                    return;
                }

                PageNavigation.provider = null;
            }
        }
    }
}