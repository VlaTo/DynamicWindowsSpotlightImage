namespace LibraProgramming.Windows.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    public interface IActionReference<in TPayload>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        void Invoke(TPayload arg);
    }
}