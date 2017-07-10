using Windows.UI.Xaml;

namespace LibraProgramming.Windows.Converters
{
    /// <summary>
    /// 
    /// </summary>
    public class Case : DependencyObject, ICase
    {
        public static readonly DependencyProperty KeyProperty;
        public static readonly DependencyProperty ValueProperty;

        /// <summary>
        /// 
        /// </summary>
        public object Key
        {
            get
            {
                return GetValue(KeyProperty);
            }
            set
            {
                SetValue(KeyProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public object Value
        {
            get
            {
                return GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        static Case()
        {
            KeyProperty = DependencyProperty
                .Register(
                    nameof(Key),
                    typeof(object),
                    typeof(Case),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
            ValueProperty = DependencyProperty
                .Register(
                    nameof(Value),
                    typeof(object),
                    typeof(Case),
                    new PropertyMetadata(null)
                );
        }
    }
}