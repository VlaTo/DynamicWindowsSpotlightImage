using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace LibraProgramming.Windows.Converters
{
    /// <summary>
    /// 
    /// </summary>
    public class CompositeConverter : DependencyObject, ICompositeConverter
    {
        /// <summary>
        /// Identifies the <see cref="PostConverter" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty PostConverterProperty;

        /// <summary>
        /// Identifies the <see cref="PostConverterProperty" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty PostConverterParameterProperty;

        /// <summary>
        /// </summary>
        public IValueConverter PostConverter
        {
            get
            {
                return (IValueConverter)GetValue(PostConverterProperty);
            }
            set
            {
                SetValue(PostConverterProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public object PostConverterParameter
        {
            get
            {
                return GetValue(PostConverterParameterProperty);
            }
            set
            {
                SetValue(PostConverterParameterProperty, value);
            }
        }

        static CompositeConverter()
        {
            PostConverterProperty = DependencyProperty
                .Register(
                    nameof(PostConverter),
                    typeof(IValueConverter),
                    typeof(CompositeConverter),
                    new PropertyMetadata(null)
                );
            PostConverterParameterProperty = DependencyProperty
                .Register(
                    nameof(PostConverterParameter),
                    typeof(object),
                    typeof(CompositeConverter),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
        }

        public virtual object Convert(object value, Type targetType, object parameter, string language)
        {
            if (null == PostConverter)
            {
                return value;
            }

            return PostConverter.Convert(value, targetType, GetParameterValue(parameter), language);
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (null == PostConverter)
            {
                return value;
            }

            return PostConverter.ConvertBack(value, targetType, GetParameterValue(parameter), language);
        }

        private object GetParameterValue(object parameter)
        {
            return DependencyProperty.UnsetValue == ReadLocalValue(PostConverterParameterProperty)
                ? parameter
                : PostConverterParameter;
        }
    }
}