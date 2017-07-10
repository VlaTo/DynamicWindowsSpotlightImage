using Windows.UI.Xaml;

namespace SpotlightImage.Window.Core
{
    public class MenuItem
    {
        public static readonly DependencyProperty TitleProperty;

        public static string GetTitle(FrameworkElement target)
        {
            return (string) target.GetValue(TitleProperty);
        }

        public static void SetTitle(FrameworkElement target, string value)
        {
            target.SetValue(TitleProperty, value);
        }

        static MenuItem()
        {
            TitleProperty = DependencyProperty
                .RegisterAttached(
                    "Title",
                    typeof(string),
                    typeof(MenuItem),
                    new PropertyMetadata(null, OnIconPropertyChanged)
                );
        }

        private static void OnIconPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
//            throw new System.NotImplementedException();
        }
    }
}