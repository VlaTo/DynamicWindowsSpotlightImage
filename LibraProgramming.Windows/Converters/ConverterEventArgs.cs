using System;
using System.Globalization;

namespace LibraProgramming.Windows.Converters
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ConverterEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public CultureInfo Culture
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        public object Parameter
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        public object Result
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public Type TargetType
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        public object Value
        {
            get;
        }

        public ConverterEventArgs(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Value = value;
            TargetType = targetType;
            Parameter = parameter;
            Culture = culture;
        }
    }
}