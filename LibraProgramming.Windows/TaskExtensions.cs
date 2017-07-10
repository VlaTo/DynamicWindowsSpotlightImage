using System.Threading.Tasks;

namespace LibraProgramming.Windows
{
    public static class TaskExtensions
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "task")]
        public static void RunAndForget(this Task task)
        {
        }
    }
}