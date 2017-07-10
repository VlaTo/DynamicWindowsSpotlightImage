using System;
using System.Collections.Generic;

namespace LibraProgramming.Windows.ServiceContainer
{
    /// <summary>
    /// 
    /// </summary>
    public interface IInstanceProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="serviceType"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        object GetInstance(Queue<ServiceTypeReference> queue, Type serviceType, string key);
    }
}