using System;
using System.Linq.Expressions;
using LibraProgramming.Windows.Infrastructure;

namespace LibraProgramming.Windows.Dependency.Tracking
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IDependencyTrackerBuilder<TModel> : IObjectBuilder<IDependencyTracker<TModel>>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        IDependentPropertyBuilder<TModel> RaiseProperty(Expression<Func<TModel, object>> expression);
    }
}