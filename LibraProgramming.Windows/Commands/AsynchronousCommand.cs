using System;
using System.Threading.Tasks;
using System.Windows.Input;
using LibraProgramming.Windows.Infrastructure;

namespace LibraProgramming.Windows.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class AsynchronousCommand : AsynchronousCommandBase, ICommand
    {
        private readonly WeakDelegate<Func<object, Task>> action;
        private readonly WeakPredicate<object> condition;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="condition"></param>
        public AsynchronousCommand(Func<object, Task> action, Predicate<object> condition = null)
        {
            this.action = new WeakDelegate<Func<object, Task>>(action);
            this.condition = null != condition ? new WeakPredicate<object>(condition) : null;
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        /// <param name="value">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public override bool CanExecute(object value)
        {
            return (null == condition || condition.Invoke(value)) && !IsExecuting;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="value">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public override void Execute(object value)
        {
            if (null == action || IsExecuting)
            {
                return;
            }

            base.Execute(value);
        }

        protected override void ExecuteInternal(object value)
        {
            action.Invoke(value);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TParam"></typeparam>
    public class AsynchronousCommand<TParam> : AsynchronousCommandBase, ICommand
    {
        private readonly WeakFunc<TParam, Task> action;
        private readonly WeakPredicate<TParam> condition;

        public AsynchronousCommand(Func<TParam, Task> action, Predicate<TParam> condition = null)
        {
            this.action = new WeakFunc<TParam, Task>(action);
            this.condition = null != condition ? new WeakPredicate<TParam>(condition) : null;
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        /// <param name="value">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public override bool CanExecute(object value)
        {
            return (null == condition || condition.Invoke((TParam)value)) && !IsExecuting;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="value">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public override void Execute(object value)
        {
            if (null == action || IsExecuting)
            {
                return;
            }

            base.Execute(value);
        }

        protected override void ExecuteInternal(object value)
        {
            action.Invoke((TParam) value);
        }
    }
}