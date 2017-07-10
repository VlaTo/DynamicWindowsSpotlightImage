using System.Threading.Tasks;

namespace LibraProgramming.Windows.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAwaitable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task WaitToComplete();
    }
}