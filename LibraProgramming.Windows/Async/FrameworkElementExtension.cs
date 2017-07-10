using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace LibraProgramming.Windows.Async
{
    public static class FrameworkElementExtension
    {
        public static async Task WaitForLoadedAsync(this FrameworkElement element)
        {
            await EventAsync.FromRoutedEvent(
                handler => element.Loaded += handler,
                handler => element.Loaded -= handler
            );
        }

        public static async Task WaitForUnload(this FrameworkElement element)
        {
            await EventAsync.FromRoutedEvent(
                handler => element.Unloaded += handler,
                handler => element.Unloaded -= handler
            );
        }
    }
}