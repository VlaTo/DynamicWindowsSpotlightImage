namespace LibraProgramming.Windows.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDeferrableRefresh
    {
        /// <summary>
        /// 
        /// </summary>
        int DeferLevel
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        void Refresh();
    }
}