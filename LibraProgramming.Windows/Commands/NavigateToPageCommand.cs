using System;
using System.Windows.Input;
using Windows.UI.Xaml;
using LibraProgramming.Windows.Infrastructure;
using LibraProgramming.Windows.Navigations;

namespace LibraProgramming.Windows.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class NavigateToPageCommand : DependencyObject, ICommand
    {
        /// <summary>
        /// Identifies the <see cref="TargetPage" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty TargetPageProperty;

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty;

        private readonly WeakEventHandler canExecuteChanged;

        /// <summary>
        /// 
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return (bool) GetValue(IsEnabledProperty);
            }
            set
            {
                SetValue(IsEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets target page type for the navigation.
        /// </summary>
        public Type TargetPage
        {
            get
            {
                return (Type) GetValue(TargetPageProperty);
            }
            set
            {
                SetValue(TargetPageProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                canExecuteChanged.AddHandler(value);
            }
            remove
            {
                canExecuteChanged.RemoveHandler(value);
            }
        }

        public NavigateToPageCommand()
        {
            canExecuteChanged = new WeakEventHandler();
        }

        static NavigateToPageCommand()
        {
            IsEnabledProperty = DependencyProperty
                .Register(
                    "IsEnabled",
                    typeof (bool),
                    typeof (NavigateToPageCommand),
                    new PropertyMetadata(true, OnIsEnabledPropertyChanged)
                );
            TargetPageProperty = DependencyProperty
                .Register(
                    "TargetPage",
                    typeof (Type),
                    typeof (NavigateToPageCommand),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
        }

        public bool CanExecute(object parameter)
        {
            return IsEnabled;
        }

        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                PageNavigation.NavigateToPage(TargetPage, parameter);
            }
        }

        private void OnIsEnabledChanged()
        {
            canExecuteChanged.Invoke(this, EventArgs.Empty);
        }

        private static void OnIsEnabledPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((NavigateToPageCommand) source).OnIsEnabledChanged();
        }
    }
}