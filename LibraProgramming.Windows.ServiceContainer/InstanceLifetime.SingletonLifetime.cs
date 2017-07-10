using System;
using System.Collections.Generic;

namespace LibraProgramming.Windows.ServiceContainer
{
    partial class InstanceLifetime
    {
        /// <summary>
        /// 
        /// </summary>
        public static Func<Factory, InstanceLifetime> Singleton
        {
            get
            {
                return factory => new SingletonLifetime(factory);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class SingletonLifetime : InstanceLifetime
        {
            private object instance;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="factory"></param>
            public SingletonLifetime(Factory factory)
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
                return instance ?? (instance = Factory.Create(queue));
            }
        }
    }
}