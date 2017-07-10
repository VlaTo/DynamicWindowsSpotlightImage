namespace LibraProgramming.Windows.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    public interface INavigationRequired
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        void Enter(object parameter);

        /// <summary>
        /// 
        /// </summary>
        void Leave();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TParam"></typeparam>
    public interface INavigationRequired<in TParam>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        void Enter(TParam parameter);
    }
}