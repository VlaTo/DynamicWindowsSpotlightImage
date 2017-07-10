namespace LibraProgramming.Windows.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    public interface IPredicateReference<in TPayload>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        bool Invoke(TPayload arg);
    }
}