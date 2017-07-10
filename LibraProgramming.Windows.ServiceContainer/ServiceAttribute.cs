using System;

namespace LibraProgramming.Windows.ServiceContainer
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ServiceAttribute : Attribute
    {
        public string Key
        {
            get;
            set;
        }
    }
}