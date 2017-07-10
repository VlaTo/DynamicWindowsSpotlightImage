using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using LibraProgramming.Windows.Async;
using LibraProgramming.Windows.Infrastructure;

namespace LibraProgramming.Windows.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class AsyncRelayCommand : IAsyncCommand, INotifyPropertyChanged
    {
        private readonly WeakEventHandler executeChanged;
        private readonly WeakEvent<TypedEventHandler<IAsyncCommand, CommandCompleteEventArgs>> complete;
        private readonly WeakEvent<PropertyChangedEventHandler> propertyChanged;
        private readonly WeakDelegate<Func<object, Task>> action;
        private readonly WeakPredicate<object> condition;
        private readonly AsyncReaderWriterLock access;
        private bool isExecuting;
        private CancellationTokenSource cts;

        /// <summary>
        /// 
        /// </summary>
        public bool IsExecuting
        {
            get
            {
                return isExecuting;
            }
            set
            {
                if (value == isExecuting)
                {
                    return;
                }

                isExecuting = value;

                propertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(IsExecuting)));
            }
        }

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public event TypedEventHandler<IAsyncCommand, CommandCompleteEventArgs> Complete
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

        /// <summary>
        /// 
        /// </summary>
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

        protected CancellationToken Cancellation
        {
            get
            {
                if (null == cts)
                {
                    throw new InvalidOperationException();
                }

                return cts.Token;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="condition"></param>
        public AsyncRelayCommand(Func<object, Task> action, Predicate<object> condition = null)
        {
            this.action = new WeakDelegate<Func<object, Task>>(action);
            this.condition = null != condition ? new WeakPredicate<object>(condition) : null;
            access = new AsyncReaderWriterLock();
            executeChanged = new WeakEventHandler();
            complete = new WeakEvent<TypedEventHandler<IAsyncCommand, CommandCompleteEventArgs>>();
            propertyChanged = new WeakEvent<PropertyChangedEventHandler>();
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        /// <param name="value">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public bool CanExecute(object value)
        {
            return false == IsExecuting && (null == condition || condition.Invoke(value));
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="value">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object value)
        {
            using (var token = access.AccquireUpgradeableReaderLock())
            {
                if (false == CanExecute(value))
                {
                    return;
                }

                executeChanged.Invoke(this, EventArgs.Empty);

                using (token.Upgrade())
                {
                    if (IsExecuting)
                    {
                        return;
                    }

                    try
                    {
                        IsExecuting = true;
                        cts = new CancellationTokenSource();

                        Task.Factory.StartNew(DoExecute, value, cts.Token).RunAndForget();
                    }
                    catch (Exception)
                    {
                        IsExecuting = false;
                        complete.Invoke(this, new CommandCompleteEventArgs(CompleteStatus.Failed));
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void RequestCancel()
        {
            if (null == cts)
            {
                throw new InvalidOperationException();
            }

            cts.Cancel();
        }

        private async Task DoExecute(object value)
        {
            if (false == action.IsAlive)
            {
                return;
            }

            Task task = null;

            try
            {
                var func = action.CreateDelegate();

                task = func.Invoke(value);

                await task.ConfigureAwait(false);
            }
            finally
            {
                using (access.AccquireWriterLock())
                {
                    IsExecuting = false;
                    cts = null;
                }

                var status = GetTaskCompleteStatus(task);

                complete.Invoke(this, new CommandCompleteEventArgs(status));
            }
        }

        private static CompleteStatus GetTaskCompleteStatus(Task task)
        {
            if (null == task)
            {
                return CompleteStatus.Failed;
            }

            switch (task.Status)
            {
                case TaskStatus.Canceled:
                    return CompleteStatus.Canceled;

                case TaskStatus.Faulted:
                    return CompleteStatus.Failed;

                case TaskStatus.RanToCompletion:
                    return CompleteStatus.Success;

                default:
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AsyncRelayCommand<TParam> : IAsyncCommand, INotifyPropertyChanged
    {
        private readonly WeakEventHandler executeChanged;
        private readonly WeakEvent<TypedEventHandler<IAsyncCommand, CommandCompleteEventArgs>> complete;
        private readonly WeakEvent<PropertyChangedEventHandler> propertyChanged;
        private readonly WeakDelegate<Func<TParam, Task>> action;
        private readonly WeakPredicate<TParam> condition;
        private readonly AsyncReaderWriterLock access;
        private CancellationTokenSource cts;
        private bool isExecuting;

        /// <summary>
        /// 
        /// </summary>
        public bool IsExecuting
        {
            get
            {
                return isExecuting;
            }
            set
            {
                if (value == isExecuting)
                {
                    return;
                }

                isExecuting = value;

                propertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(IsExecuting)));
            }
        }

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public event TypedEventHandler<IAsyncCommand, CommandCompleteEventArgs> Complete
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

        /// <summary>
        /// 
        /// </summary>
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

        protected CancellationToken Cancellation
        {
            get
            {
                if (null == cts)
                {
                    throw new InvalidOperationException();
                }

                return cts.Token;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="condition"></param>
        public AsyncRelayCommand(Func<TParam, Task> action, Predicate<TParam> condition = null)
        {
            this.action = new WeakDelegate<Func<TParam, Task>>(action);
            this.condition = null != condition ? new WeakPredicate<TParam>(condition) : null;
            access = new AsyncReaderWriterLock();
            executeChanged = new WeakEventHandler();
            complete = new WeakEvent<TypedEventHandler<IAsyncCommand, CommandCompleteEventArgs>>();
            propertyChanged = new WeakEvent<PropertyChangedEventHandler>();
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        /// <param name="value">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public bool CanExecute(object value)
        {
            return false == IsExecuting && (null == condition || condition.Invoke((TParam) value));
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="value">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object value)
        {
            using (var token = access.AccquireUpgradeableReaderLock())
            {
                if (false == CanExecute(value))
                {
                    return;
                }

                executeChanged.Invoke(this, EventArgs.Empty);

                using (token.Upgrade())
                {
                    if (IsExecuting)
                    {
                        return;
                    }

                    IsExecuting = true;
                    cts = new CancellationTokenSource();

                    Task.Factory
                        .StartNew(DoExecute, value, cts.Token)
                        .Unwrap()
                        .RunAndForget();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void RequestCancel()
        {
            if (null == cts)
            {
                throw new InvalidOperationException();
            }

            cts.Cancel();
        }

        private async Task DoExecute(object arg)
        {
            if (false == action.IsAlive)
            {
                return;
            }

            var func = action.CreateDelegate();
            var task = func.Invoke((TParam) arg);

            await task.ConfigureAwait(false);

            using (access.AccquireWriterLock())
            {
                IsExecuting = false;
                cts = null;
            }

            var status = GetTaskCompleteStatus(task);

            complete.Invoke(this, new CommandCompleteEventArgs(status));
        }

        private CompleteStatus GetTaskCompleteStatus(Task task)
        {
            switch (task.Status)
            {
                case TaskStatus.Canceled:
                    return CompleteStatus.Canceled;

                case TaskStatus.Faulted:
                    return CompleteStatus.Failed;

                case TaskStatus.RanToCompletion:
                    return CompleteStatus.Success;

                default:
                    {
                        throw new InvalidOperationException();
                    }
            }
        }
    }
}