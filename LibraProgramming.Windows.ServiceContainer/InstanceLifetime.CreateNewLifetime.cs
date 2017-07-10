using System;
using System.Collections.Generic;

namespace LibraProgramming.Windows.ServiceContainer
{
    partial class InstanceLifetime
    {
        /// <summary>
        /// 
        /// </summary>
        public static Func<Factory, InstanceLifetime> CreateNew
        {
            get
            {
                return factory => new CreateNewLifetime(factory);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class CreateNewLifetime : InstanceLifetime
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="factory"></param>
            public CreateNewLifetime(Factory factory)
                : base(factory)
            {
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="queue"></param>
            /// <returns></returns>
            public override object ResolveInstance(Queue<ServiceTypeReference> queue)
            {
                return Factory.Create(queue);
            }
        }
    }
}