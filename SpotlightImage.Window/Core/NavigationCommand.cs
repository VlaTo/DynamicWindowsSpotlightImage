using System;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using LibraProgramming.Windows.Infrastructure;

namespace SpotlightImage.Window.Core
{
    public sealed class NavigationCommand : DependencyObject, ICommand
    {
        public static readonly DependencyProperty NavigationFrameProperty;
        public static readonly DependencyProperty TargetPageProperty;

        private bool canExecute;
        private readonly WeakEventHandler executeChanged;

        public Frame NavigationFrame
        {
            get
            {
                return (Frame) GetValue(NavigationFrameProperty);
            }
            set
            {
                SetValue(NavigationFrameProperty, value);
            }
        }

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

        public event EventHandler CanExecuteChanged
        {
            add
            {
                executeChanged.AddHandler(value);
            }
            remove
            {
                executeChanged.RemoveHandler(value);
            }
        }

        public NavigationCommand()
        {
            executeChanged = new WeakEventHandler();
        }

        static NavigationCommand()
        {
            NavigationFrameProperty = DependencyProperty
                .Register(
                    nameof(NavigationFrame),
                    typeof(Frame),
                    typeof(NavigationCommand),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
            TargetPageProperty = DependencyProperty
                .Register(
                    nameof(TargetPage),
                    typeof(Type),
                    typeof(NavigationCommand),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
        }

        public bool CanExecute(object parameter)
        {
            return null != TargetPage && null != NavigationFrame;
        }

        public void Execute(object parameter)
        {
            var flag = CanExecute(parameter);

            if (canExecute != flag)
            {
                canExecute = flag;
                executeChanged.Invoke(this, EventArgs.Empty);
            }

            if (false == flag)
            {
                return;
            }

            if (null == parameter)
            {
                NavigationFrame.Navigate(TargetPage);
            }
            else
            {
                NavigationFrame.Navigate(TargetPage, parameter);
            }
        }
    }
}