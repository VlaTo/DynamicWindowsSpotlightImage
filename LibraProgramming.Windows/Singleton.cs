using System;
using System.Collections.Concurrent;

namespace LibraProgramming.Windows
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    public static class Singleton<TInstance>
        where TInstance : new()
    {
        private static readonly ConcurrentDictionary<Type, TInstance> instances;

        /// <summary>
        /// 
        /// </summary>
        public static TInstance Instance
        {
            get
            {
                return instances.GetOrAdd(typeof (TInstance), type => new TInstance());
            }
        }

        static Singleton()
        {
            instances = new ConcurrentDictionary<Type, TInstance>();
        } 
    }
}