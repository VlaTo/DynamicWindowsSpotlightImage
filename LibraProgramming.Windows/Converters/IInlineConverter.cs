using System;
using Windows.UI.Xaml.Data;

namespace LibraProgramming.Windows.Converters
{
    /// <summary>
    /// 
    /// </summary>
    public interface IInlineConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        event EventHandler<ConverterEventArgs> Converting;

        /// <summary>
        /// 
        /// </summary>
        event EventHandler<ConverterEventArgs> ConvertingBack;
    }
}