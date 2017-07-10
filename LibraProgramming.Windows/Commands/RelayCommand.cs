using System;
using System.Windows.Input;
using LibraProgramming.Windows.Infrastructure;

namespace LibraProgramming.Windows.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly WeakDelegate<Action> action;
        private readonly WeakFunc<bool> canExecute;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="canExecute"></param>
        public RelayCommand(Action action, Func<bool> canExecute = null)
        {
            this.action = new WeakDelegate<Action>(action);
            this.canExecute = null != canExecute ? new WeakFunc<bool>(canExecute) : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unused"></param>
        /// <returns></returns>
        public bool CanExecute(object unused)
        {
            return null == canExecute || canExecute.Invoke();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unused"></param>
        public void Execute(object unused)
        {
            action?.Invoke();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TParam"></typeparam>
    public class RelayCommand<TParam> : ICommand
    {
        private readonly WeakDelegate<Action<TParam>> action;
        private readonly WeakPredicate<TParam> canExecute;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="canExecute"></param>
        public RelayCommand(Action<TParam> action, Predicate<TParam> canExecute = null)
        {
            this.action = new WeakDelegate<Action<TParam>>(action);
            this.canExecute = null != canExecute ? new WeakPredicate<TParam>(canExecute) : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool CanExecute(object value)
        {
            return null == canExecute || canExecute.Invoke((TParam)value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void Execute(object value)
        {
            action?.Invoke(value);
        }
    }
}