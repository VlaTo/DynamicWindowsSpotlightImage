using System.Threading.Tasks;

namespace LibraProgramming.Windows.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEventSubscription
    {
        /// <summary>
        /// 
        /// </summary>
        SubscriptionToken Token
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        Task ExecuteAsync(params object[] args);
    }
}