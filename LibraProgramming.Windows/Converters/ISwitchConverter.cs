using System.Collections.Generic;
using Windows.UI.Xaml.Data;

namespace LibraProgramming.Windows.Converters
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICase
    {
        /// <summary>
        /// 
        /// </summary>
        object Key
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        object Value
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CaseSet : List<ICase>
    {
        public static readonly object Undefined = new object();
    }

    /// <summary>
    /// 
    /// </summary>
    public interface ISwitchConverter : IValueConverter
    {
        CaseSet Cases
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        object Default
        {
            get;
            set;
        }
    }
}