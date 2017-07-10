using System.Threading.Tasks;

namespace LibraProgramming.Windows.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICleanupRequired
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task CleanupAsync();
    }
}