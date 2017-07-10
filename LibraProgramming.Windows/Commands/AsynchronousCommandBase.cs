using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using LibraProgramming.Windows.Infrastructure;

namespace LibraProgramming.Windows.Commands
{
    /// <summary>
    /// Enumeration for command completition status.
    /// </summary>
    public enum CommandCompletion
    {
        Cancelled = -1,
        Unknown,
        Success
    }

    /// <summary>
    /// Provides data for <see cref="AsynchronousCommandBase{TParam}.CanExecuteChanged" /> event.
    /// </summary>
    public class CompleteCommandEventArgs : EventArgs
    {
        public CommandCompletion Completion
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.EventArgs"/> class.
        /// </summary>
        public CompleteCommandEventArgs(CommandCompletion completion)
        {
            Completion = completion;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void CompleteCommandEventHandler(object sender, CompleteCommandEventArgs e);

    public abstract class AsynchronousCommandBase : ICancelRequired, INotifyPropertyChanged
    {
        private readonly WeakEvent<CompleteCommandEventHandler> complete;
        private readonly WeakEvent<EventHandler> canExecute;
        private readonly WeakEvent<PropertyChangedEventHandler> propertyChanged;
        private CancellationTokenSource tokenSource;
        private bool isExecuting;

        public bool IsExecuting
        {
            get
            {
                return isExecuting;
            }
            protected set
            {
                if (value == isExecuting)
                {
                    return;
                }

                isExecuting = value;

                propertyChanged.Invoke(this, new PropertyChangedEventArgs("IsExecuting"));
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                canExecute.AddHandler(value);
            }
            remove
            {
                canExecute.RemoveHandler(value);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                propertyChanged.AddHandler(value);
            }
            remove
            {
                propertyChanged.RemoveHandler(value);
            }
        }

        public event CompleteCommandEventHandler Complete
        {
            add
            {
                complete.AddHandler(value);
            }
            remove
            {
                complete.RemoveHandler(value);
            }
        }

        protected AsynchronousCommandBase()
        {
            complete = new WeakEvent<CompleteCommandEventHandler>();
            canExecute = new WeakEvent<EventHandler>();
            propertyChanged = new WeakEvent<PropertyChangedEventHandler>();
        }

        public abstract bool CanExecute(object value);

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="value">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public virtual void Execute(object value)
        {
            canExecute.Invoke(this, EventArgs.Empty);

            if (!CanExecute(value))
            {
                return;
            }

            tokenSource = new CancellationTokenSource();

            Task
                .Run((Action) BeforeCommandAction)
                .ContinueWith(DoExecute, value, tokenSource.Token)
                .ContinueWith(AfterCommandAction, value);
        }

        public void RequestCancel()
        {
            tokenSource.Cancel();
        }

        protected abstract void ExecuteInternal(object value);

        protected void DoExecute(Task task, object value)
        {
            ExecuteInternal(value);
        }

        protected virtual void BeforeCommandAction()
        {
            IsExecuting = true;
        }

        protected virtual void AfterCommandAction(Task task, object state)
        {
            IsExecuting = false;

            var completion = CommandCompletion.Unknown;

            if (task.IsCanceled)
            {
                completion = CommandCompletion.Cancelled;
            }
            else if (task.IsCompleted)
            {
                completion = CommandCompletion.Success;
            }

            complete.Invoke(this, new CompleteCommandEventArgs(completion));
        }
    }
}