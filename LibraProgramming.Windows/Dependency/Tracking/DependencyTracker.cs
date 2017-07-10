using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace LibraProgramming.Windows.Dependency.Tracking
{
    /// <summary>
    /// Dependency tacker class.
    /// </summary>
    public partial class DependencyTracker
    {
        /// <summary>
        /// Creates new <see cref="DependencyTracker" /> and configure them.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="configurator"></param>
        /// <returns></returns>
        public static IDependencyTracker<TModel> Create<TModel>(Action<IDependencyTrackerBuilder<TModel>> configurator)
        {
            if (null == configurator)
            {
                throw new ArgumentNullException(nameof(configurator));
            }

            EnsureTypeTrackable(typeof(TModel));

            var builder = new DependencyTrackerBuilder<TModel>();

            configurator.Invoke(builder);

            return builder.Construct();
        }

        private static void EnsureTypeTrackable(Type targeType)
        {
            if (typeof(INotifyPropertyChanged).IsAssignableFrom(targeType))
            {
                return;
            }

            throw new Exception();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public sealed class DependencyTracker<TModel> : DependencyTracker, IDependencyTracker<TModel>
    {
        internal IList<PropertyDependency<TModel>> Dependency
        {
            get;
        }

        internal DependencyTracker(IList<PropertyDependency<TModel>> dependency)
        {
            Dependency = dependency;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        IDependencyTracketSubscription IDependencyTracker<TModel>.Subscribe(TModel target)
        {
            if (null == target)
            {
                throw new ArgumentNullException(nameof(target));
            }

            var subsription = new DependencySubscription<TModel>(this);

            subsription.Subscribe(target);

            return subsription;
        }
    }
}