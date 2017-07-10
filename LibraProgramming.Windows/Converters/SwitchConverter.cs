using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace LibraProgramming.Windows.Converters
{
    /// <summary>
    /// 
    /// </summary>
    [ContentProperty(Name = nameof(Cases))]
    public class SwitchConverter : CompositeConverter, ISwitchConverter
    {
        public static readonly DependencyProperty DefaultProperty;

        /// <summary>
        /// 
        /// </summary>
        public CaseSet Cases
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        public object Default
        {
            get
            {
                return GetValue(DefaultProperty);
            }
            set
            {
                SetValue(DefaultProperty, value);
            }
        }

        public SwitchConverter()
        {
            Cases = new CaseSet();
        }

        static SwitchConverter()
        {
            DefaultProperty = DependencyProperty
                .Register(
                    nameof(Default),
                    typeof(object),
                    typeof(SwitchConverter),
                    new PropertyMetadata(CaseSet.Undefined)
                );
        }

        public override object Convert(object value, Type targetType, object parameter, string language)
        {
            var @case = Cases.FirstOrDefault(pair => KeyEquals(pair.Key, value));
            var result = null == @case ? Default : @case.Value;

            return CaseSet.Undefined == result
                ? base.Convert(value, targetType, parameter, language)
                : result;
        }

        private bool KeyEquals(object key, object value)
        {
            if (ReferenceEquals(key, null) && ReferenceEquals(value, null))
            {
                return true;
            }

//            if()
            return Object.Equals(key, value);

        }
    }
}