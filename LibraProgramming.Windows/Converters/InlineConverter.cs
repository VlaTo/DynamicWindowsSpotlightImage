using System;
using System.Globalization;

namespace LibraProgramming.Windows.Converters
{
    /// <summary>
    /// 
    /// </summary>
    public class InlineConverter : CompositeConverter, IInlineConverter
    {
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<ConverterEventArgs> Converting;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<ConverterEventArgs> ConvertingBack;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public override object Convert(object value, Type targetType, object parameter, string language)
        {
            var cultureinfo = new CultureInfo(language);
            var handler = Converting;
            var e = new ConverterEventArgs(value, targetType, parameter, cultureinfo);

            object argument;

            if (null != handler)
            {
                handler.Invoke(this, e);
                argument = e.Result;
            }
            else
            {
                argument = value;
            }

            return base.Convert(argument, targetType, parameter, language);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public override object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var cultureinfo = new CultureInfo(language);
            var handler = ConvertingBack;
            var e = new ConverterEventArgs(value, targetType, parameter, cultureinfo);

            object argument;

            if (null != handler)
            {
                handler.Invoke(this, e);
                argument = e.Result;
            }
            else
            {
                argument = value;
            }

            return base.ConvertBack(argument, targetType, parameter, language);
        }
    }
}