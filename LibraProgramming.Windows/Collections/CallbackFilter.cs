using System;
using Windows.UI.Xaml;

namespace LibraProgramming.Windows.Collections
{
    /// <summary>
    /// 
    /// </summary>
    public class FilterCallbackEventArgs : EventArgs
    {
        public object Item
        {
            get;
        }

        public bool IsPasses
        {
            get;
            set;
        }

        public FilterCallbackEventArgs(object item)
        {
            Item = item;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CallbackFilter : DependencyObject, ICollectionFilter
    {
        public event EventHandler<FilterCallbackEventArgs> ItemFilter;

        public bool CanPassFilter(SourceCollectionView sender, object item)
        {
            var args = new FilterCallbackEventArgs(item);

            ItemFilter?.Invoke(sender, args);

            return args.IsPasses;
        }
    }
}