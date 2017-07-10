using System;
using System.Diagnostics.Contracts;

namespace LibraProgramming.Windows.Navigations
{
    public class PageNavigationException : Exception
    {
        private readonly string message;

        public override string Message => message ?? base.Message;

        public Type TargetPage
        {
            get;
        }

        public PageNavigationException(Type targetPage)
        {
            Contract.Requires(null != targetPage);

            message = String.Format("Error navigating to page: \"{0}\"", targetPage.FullName);
            TargetPage = targetPage;
        }

        public PageNavigationException(Type targetPage, string message)
        {
            Contract.Requires(null != targetPage);
            Contract.Requires(null != message);

            this.message = String.Format("Error navigating to page: \"{0}\" with message: \"{1}\"", targetPage.FullName, message);
            TargetPage = targetPage;
        }
    }
}