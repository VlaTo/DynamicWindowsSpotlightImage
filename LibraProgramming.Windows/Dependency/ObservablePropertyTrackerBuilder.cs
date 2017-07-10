using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LibraProgramming.Windows.Dependency
{
    internal class ObservablePropertyTrackerBuilder<TModel> : IObservablePropertyTrackerBuilder<TModel>
        where TModel : ObservableModel
    {
        private readonly HashSet<ObservingPropertyBuilder<TModel>> builders;

        internal ObservablePropertyTrackerBuilder()
        {
            builders = new HashSet<ObservingPropertyBuilder<TModel>>();
        }

        public IDependentObservablePropertyBuilder<TModel> RaiseProperty(Expression<Func<TModel, object>> expression)
        {
            if (null == expression)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var path = PropertyPath.Parse(expression);
            var property = ObservableProperty.CreateFor(typeof(TModel), path);

            if (null == property)
            {
                throw new ObservablePropertyTrackerException();
            }

            var builder = new ObservingPropertyBuilder<TModel>(property);

            builders.Add(builder);

            return builder;
        }

        public IObservablePropertyTracker<TModel> Construct()
        {
            var dependencies = new List<ObservablePropertyDependency<TModel>>();

            foreach (var builder in builders)
            {
                var dependency = dependencies.FirstOrDefault(dep => builder.PropertyPath.Equals(dep.PropertyPath));

                if (null != dependency)
                {
                    throw new ObservablePropertyTrackerException();
                }

                dependency = builder.Construct();

                dependencies.Add(dependency);
            }

            return new ObservablePropertyTracker<TModel>(dependencies);
        }
    }
}