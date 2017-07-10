namespace LibraProgramming.Windows.Infrastructure
{
    /// <summary>
    /// Manages a delayed operations.
    /// </summary>
    public interface IDeferred
    {
        /// <summary>
        /// Notifies the caller that the peer has completed and is ready to continue.
        /// </summary>
        void Complete();
    }
}