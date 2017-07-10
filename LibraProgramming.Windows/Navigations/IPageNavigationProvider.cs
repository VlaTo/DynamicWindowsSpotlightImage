using System;

namespace LibraProgramming.Windows.Navigations
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPageNavigationProvider
    {
        bool CanGoBack
        {
            get;
        }

        bool CanGoForward
        {
            get;
        }

        void GoBack();

        void GoForward();

        bool NavigateToPage(Type targetPage, object parameter);
    }
}