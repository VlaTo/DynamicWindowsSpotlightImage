using System.Collections.Generic;

namespace LibraProgramming.Windows.ServiceContainer
{
    /// <summary>
    /// 
    /// </summary>
    public abstract partial class InstanceLifetime
    {
        protected Factory Factory
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        public abstract object ResolveInstance(Queue<ServiceTypeReference> queue);

        protected InstanceLifetime(Factory factory)
        {
            Factory = factory;
        }
    }
}