using Windows.UI.Xaml.Data;

namespace LibraProgramming.Windows.Converters
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICompositeConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        IValueConverter PostConverter
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        object PostConverterParameter
        {
            get;
            set;
        }
    }
}