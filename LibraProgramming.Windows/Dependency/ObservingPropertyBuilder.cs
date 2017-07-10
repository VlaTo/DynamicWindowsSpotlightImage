using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using LibraProgramming.Windows.Infrastructure;

namespace LibraProgramming.Windows.Dependency
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    internal class ObservingPropertyBuilder<TModel> : IDependentObservablePropertyBuilder<TModel>, IDependecyObservablePropertyBuilder<TModel>, IObjectBuilder<ObservablePropertyDependency<TModel>>
        where TModel : ObservableModel
    {
        private readonly ObservableProperty property;

        /// <summary>
        /// 
        /// </summary>
        public PropertyPath PropertyPath => PropertyPath.Parse<TModel>(property.Key.PropertyName);

        /// <summary>
        /// 
        /// </summary>
        public Func<TModel, object> Calculator
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public HashSet<PropertyPath> DependencyProperties
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        public ObservingPropertyBuilder(ObservableProperty property)
        {
            this.property = property;
            DependencyProperties = new HashSet<PropertyPath>(PropertyPathComparer.Ordinal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ObservablePropertyDependency<TModel> Construct()
        {
//            var calculator = CreateUpdateAction();
            var dependency = new ObservablePropertyDependency<TModel>(PropertyPath, Calculator);

            foreach (var path in DependencyProperties)
            {
                if (false == dependency.DependencyProperties.Contains(path))
                {
                    dependency.DependencyProperties.Add(path);
                    continue;
                }

                throw new ObservablePropertyTrackerException();
            }

            return dependency;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="calculator"></param>
        /// <returns></returns>
        IDependecyObservablePropertyBuilder<TModel> IDependentObservablePropertyBuilder<TModel>.CalculatedBy(Func<TModel, object> calculator)
        {
            if (null == calculator)
            {
                throw new ArgumentNullException(nameof(calculator));
            }

            if (null != Calculator)
            {
                throw new ArgumentException("", nameof(calculator));
            }

            Calculator = calculator;

            return this;
        }

        IDependecyObservablePropertyBuilder<TModel> IDependecyObservablePropertyBuilder<TModel>.WhenChanged(Expression<Func<TModel, object>> expression)
        {
            if (null == expression)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var path = PropertyPath.Parse(expression);

            if (DependencyProperties.Contains(path))
            {
                throw new ArgumentException(nameof(expression));
            }

            DependencyProperties.Add(path);

            return this;
        }
    }
}