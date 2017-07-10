using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LibraProgramming.Windows.Dependency.Tracking
{
    public partial class DependencyTracker
    {
        private class DependencyTrackerBuilder<TModel> : IDependencyTrackerBuilder<TModel>
        {
            private readonly HashSet<DependentPropertyBuilder<TModel>> builders;

            public DependencyTrackerBuilder()
            {
                builders = new HashSet<DependentPropertyBuilder<TModel>>();
            }

            public IDependentPropertyBuilder<TModel> RaiseProperty(Expression<Func<TModel, object>> expression)
            {
                if (null == expression)
                {
                    throw new ArgumentNullException(nameof(expression));
                }

                var path = PropertyPath.Parse(expression);
                var builder = new DependentPropertyBuilder<TModel>(path);

                builders.Add(builder);

                return builder;
            }

            public IDependencyTracker<TModel> Construct()
            {
                var dependencies = new List<PropertyDependency<TModel>>(); 

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

                return new DependencyTracker<TModel>(dependencies);
            }

            /*private static Action<T, U> BuildSetter<U>(Expression<Func<T, U>> dependentProperty)
            {
                Debug.Assert(dependentProperty.Body != null);

                var memberExpression = dependentProperty.Body as MemberExpression;
                if (memberExpression == null)
                    ThrowNotSupportedExpressionForDependentProperty(dependentProperty.Body);

                if (!(memberExpression.Expression is ParameterExpression))
                    ThrowNotSupportedExpressionForDependentProperty(memberExpression);

                var objectParameter = Expression.Parameter(typeof(T), "obj");
                var assignParameter = Expression.Parameter(typeof(U), "val");
                var property = Expression.PropertyOrField(objectParameter, memberExpression.Member.Name);
                var lambda = Expression.Lambda<Action<T, U>>(Expression.Assign(property, assignParameter), objectParameter, assignParameter);
                Debug.WriteLine(lambda);
                return lambda.Compile();
            }*/

            /*private static Func<object, object> BuildGetter(Type parameterType, string propertyOrFieldName)
            {
                var parameter = Expression.Parameter(typeof(object), "obj");
                var convertedParameter = Expression.Convert(parameter, parameterType);
                var propertyGetter = Expression.PropertyOrField(convertedParameter, propertyOrFieldName);

                Debug.WriteLine(propertyGetter);

                var lambdaExpression = Expression.Lambda<Func<object, object>>(Expression.Convert(propertyGetter, typeof(object)), parameter);
                Debug.WriteLine(lambdaExpression);
                return lambdaExpression.Compile();

            }*/

            /*private static PathItemBase<T> BuildPath(Expression<Func<T, object>> pathExpession, Action<T> calculateAndSet)
            {
                var convertExpression = pathExpession.Body as UnaryExpression;
                if (convertExpression != null &&
                    (convertExpression.NodeType != ExpressionType.Convert || convertExpression.Type != typeof(object)))
                    throw new NotSupportedException(string.Format(
                        "Unary expression {0} is not supported. Only \"convert to object\" expression is allowed in the end of path.", convertExpression));

                var currentExpression = convertExpression != null ? convertExpression.Operand : pathExpession.Body;

                PathItemBase<T> rootPathItem = null;

                while (!(currentExpression is ParameterExpression))
                {
                    var methodCall = currentExpression as MethodCallExpression;
                    if (methodCall != null)
                    {
                        if (!methodCall.Method.IsGenericMethod || !methodCall.Method.GetGenericMethodDefinition().Equals(CollectionExtensions.EachElementMethodInfo))
                            throw new NotSupportedException(string.Format("Call of method {0} is not supported. Only {1} call is supported for collections in path",
                                                                          methodCall.Method, CollectionExtensions.EachElementMethodInfo));

                        rootPathItem = new CollectionPathItem<T>(rootPathItem, rootPathItem == null ? calculateAndSet : null);

                        var methodCallArgument = methodCall.Arguments.Single();
                        currentExpression = methodCallArgument;
                        continue;
                    }

                    var memberExpression = currentExpression as MemberExpression;
                    if (memberExpression == null)
                        throw new NotSupportedException(string.Format("Expected expression is member expression. Expression {0} is not supported.", currentExpression));

                    var property = memberExpression.Member;
                    var compiledGetter = BuildGetter(memberExpression.Expression.Type, property.Name);

                    rootPathItem = new PropertyPathItem<T>(compiledGetter, property.Name, rootPathItem, rootPathItem == null ? calculateAndSet : null);

                    currentExpression = memberExpression.Expression;
                }

                //The chain doesn't contain any element (i.e. the expression contains only root object root => root)
                if (rootPathItem == null)
                    throw new NotSupportedException(string.Format("The path {0} is too short. It contains a root object only.", pathExpession));

                rootPathItem = new PropertyPathItem<T>(o => o, string.Empty, rootPathItem, null);

                return rootPathItem;
            }*/

            /*private static Action<TModel, object> Temp(PropertyPath path, Func<TModel, object> updater)
            {
                var modelType = typeof (TModel);
                var ret = modelType.GetProperty(path.Name);
                var model = Expression.Parameter(modelType, "model");
                var value = Expression.Parameter(ret.PropertyType, "value");
                var temp1 = updater.GetMethodInfo();
                var temp2 = Expression.Call(null, temp1, model);
                var temp3 = Expression.Convert(temp2, ret.PropertyType);
                var temp = Expression.Assign(value, temp3);
                //var property = Expression.Property(model, path.Name);
                //var t1 = typeof (Action<>).MakeGenericType(modelType, ret.PropertyType);
                //var assign = Expression.Assign(property, temp);
                //var lambda = Expression.Lambda(t1, assign, value);

                //return (Action<TModel, object>) lambda.Compile();
                return null;
            }*/
        }
    }
}