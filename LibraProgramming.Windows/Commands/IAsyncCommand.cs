using System;
using System.Windows.Input;
using Windows.Foundation;

namespace LibraProgramming.Windows.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public enum CompleteStatus
    {
        /// <summary>
        /// 
        /// </summary>
        Failed=-1,

        /// <summary>
        /// 
        /// </summary>
        Success,

        /// <summary>
        /// 
        /// </summary>
        Canceled
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class CommandCompleteEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public CompleteStatus CompleteStatus
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="completeStatus"></param>
        public CommandCompleteEventArgs(CompleteStatus completeStatus)
        {
            CompleteStatus = completeStatus;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IAsyncCommand : ICommand
    {
        /// <summary>
        /// 
        /// </summary>
        bool IsExecuting
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        event TypedEventHandler<IAsyncCommand, CommandCompleteEventArgs> Complete;

        /// <summary>
        /// 
        /// </summary>
        void RequestCancel();
    }
}