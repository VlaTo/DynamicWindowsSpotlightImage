using System;
using System.Collections.Generic;
using System.Reflection;

namespace LibraProgramming.Windows.ServiceContainer
{
    /// <summary>
    /// Implementation of the IOC pattern.
    /// </summary>
    public sealed partial class ServiceLocator : IServiceLocator, IInstanceProvider, IServiceRegistry
    {
        private static readonly Lazy<ServiceLocator> instance;
        private static readonly object sync;

        private readonly Dictionary<Type, InstanceCollection> registration;

        /// <summary>
        /// Gets instance of the <see cref="ServiceLocator" /> class.
        /// </summary>
        public static ServiceLocator Current => instance.Value;

        private ServiceLocator()
        {
            registration = new Dictionary<Type, InstanceCollection>();
        }

        static ServiceLocator()
        {
            sync = new object();
            instance = new Lazy<ServiceLocator>(() => new ServiceLocator());
        }

        #region Service Locator implementation

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public object GetService(Type serviceType)
        {
            return GetInstance(serviceType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetInstance(Type serviceType, string key = null)
        {
            if (null == serviceType)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            var queue = new Queue<ServiceTypeReference>();

            return GetInstanceInternal(queue, serviceType, key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="serviceType"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        object IInstanceProvider.GetInstance(Queue<ServiceTypeReference> queue, Type serviceType, string key)
        {
            return GetInstanceInternal(queue, serviceType, key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public TService GetInstance<TService>(string key = null)
        {
            return (TService) GetInstance(typeof (TService), key);
        }

        #endregion

        #region Service Registration

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="lifetime"></param>
        /// <param name="key"></param>
        /// <param name="createimmediate"></param>
        public void Register(Type service, Func<Factory, InstanceLifetime> lifetime = null, string key = null, bool createimmediate = false)
        {
            if (null == service)
            {
                throw new ArgumentNullException(nameof(service));
            }

            var ti = service.GetTypeInfo();

            if (ti.IsAbstract || ti.IsInterface)
            {
//                Throw.UnsupportedServiceType(service);
                throw new NotSupportedException();
            }

            RegisterService(service, new TypeFactory(this, service), lifetime, key, createimmediate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="impl"></param>
        /// <param name="lifetime"></param>
        /// <param name="key"></param>
        /// <param name="createimmediate"></param>
        public void Register(Type service, Type impl, Func<Factory, InstanceLifetime> lifetime = null, string key = null, bool createimmediate = false)
        {
            if (null == service)
            {
                throw new ArgumentNullException(nameof(service));
            }

            var ti = service.GetTypeInfo();

            if (!ti.IsAbstract && !ti.IsInterface)
            {
//                Throw.UnsupportedServiceType(service);
                throw new NotSupportedException();
            }

            if (null == impl)
            {
                throw new ArgumentNullException(nameof(impl));
            }

            ti = impl.GetTypeInfo();

            if (ti.IsAbstract || ti.IsInterface)
            {
//                Throw.UnsupportedServiceType(impl);
                throw new NotSupportedException();
            }

            RegisterService(service, new TypeFactory(this, impl), lifetime, key, createimmediate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="factory"></param>
        /// <param name="lifetime"></param>
        /// <param name="key"></param>
        /// <param name="createimmediate"></param>
        public void Register<TService>(Func<TService> factory, Func<Factory, InstanceLifetime> lifetime = null, string key = null, bool createimmediate = false)
        {
            RegisterService(typeof (TService), new CreatorFactory<TService>(this, factory), lifetime, key, createimmediate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="lifetime"></param>
        /// <param name="key"></param>
        /// <param name="createimmediate"></param>
        public void Register<TService>(Func<Factory, InstanceLifetime> lifetime = null, string key = null, bool createimmediate = false) where TService : class 
        {
            Register(typeof (TService), lifetime, key, createimmediate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TConcrete"></typeparam>
        /// <param name="lifetime"></param>
        /// <param name="key"></param>
        /// <param name="createimmediate"></param>
        public void Register<TService, TConcrete>(Func<Factory, InstanceLifetime> lifetime = null, string key = null, bool createimmediate = false)
            where TConcrete : class, TService
        {
            Register(typeof (TService), typeof (TConcrete), lifetime, key, createimmediate);
        }

        private object GetInstanceInternal(Queue<ServiceTypeReference> queue, Type serviceType, string key = null)
        {
            lock (sync)
            {
                ServiceLocator.InstanceCollection collection;

                if (!registration.TryGetValue(serviceType, out collection))
                {
//                    Throw.MissingServiceRegistration(serviceType, nameof(serviceType));
                    throw new ServiceLocatorException();
                }

                return collection[key].ResolveInstance(queue);
            }
        }

        #endregion

        private void RegisterService(Type service, Factory factory, Func<Factory, InstanceLifetime> lifetime, string key, bool createimmediate)
        {
            lock (sync)
            {
                ServiceLocator.InstanceCollection collection;

                if (!registration.TryGetValue(service, out collection))
                {
                    collection = new ServiceLocator.InstanceCollection();
                    registration.Add(service, collection);
                }
                else if (null == key)
                {
//                    Throw.MissingServiceRegistration(service, nameof(service));
                    throw new ServiceLocatorException();
                }

                var manager = lifetime ?? InstanceLifetime.Singleton;

                collection[key] = manager(factory);

                if (createimmediate)
                {
                    var queue = new Queue<ServiceTypeReference>();
                    collection[key].ResolveInstance(queue);
                }
            }
        }
    }
}