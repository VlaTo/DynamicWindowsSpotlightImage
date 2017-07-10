using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using LibraProgramming.Windows.Infrastructure;

namespace LibraProgramming.Windows.Dependency.Tracking
{
    internal class DependentPropertyBuilder<TModel> : IDependentPropertyBuilder<TModel>, IObjectBuilder<PropertyDependency<TModel>>
    {
        public PropertyPath PropertyPath
        {
            get;
        }

        public HashSet<PropertyPath> DependencyProperties
        {
            get;
        }

        public Func<TModel, object> Calculator
        {
            get;
            private set;
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:System.Object"/>.
        /// </summary>
        public DependentPropertyBuilder(PropertyPath path)
        {
            PropertyPath = path;
            DependencyProperties = new HashSet<PropertyPath>(PropertyPathComparer.Ordinal);
        }

        public PropertyDependency<TModel> Construct()
        {
            var calculator = CreateUpdateAction();
            var dependency = new PropertyDependency<TModel>(PropertyPath, calculator);

            foreach (var path in DependencyProperties)
            {
                if (!dependency.DependencyProperties.Contains(path))
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
        /// <param name="expression"></param>
        /// <returns></returns>
        IDependencyPropertySelector<TModel> IDependencyPropertySelector<TModel>.WhenPropertyChanged(Expression<Func<TModel, object>> expression)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="calculator"></param>
        /// <returns></returns>
        IDependencyPropertySelector<TModel> IDependentPropertyBuilder<TModel>.CalculatedBy(Func<TModel, object> calculator)
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

        private Action<TModel> CreateUpdateAction()
        {
            try
            {
                var type = typeof(TModel);
                var value = Expression.Parameter(typeof(object), "value");
                var model = Expression.Parameter(type, "model");
                var lambda = Expression
                    .Lambda<Action<TModel, object>>(
                        Expression.Assign(
                            Expression.Property(model, PropertyPath.Name),
                            Expression.Convert(value, type.GetProperty(PropertyPath.Name).PropertyType)),
                        model,
                        value);

                var func = lambda.Compile();

                return target =>
                {
                    var temp = Calculator.Invoke(target);
                    func.Invoke(target, temp);
                };
            }
            catch (Exception exception)
            {
                throw new ObservablePropertyTrackerException("", exception);
            }
        }
    }
}