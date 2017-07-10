using System;
using System.Linq.Expressions;
using LibraProgramming.Windows.Infrastructure;

namespace LibraProgramming.Windows.Dependency
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IObservablePropertyTrackerBuilder<TModel> : IObjectBuilder<IObservablePropertyTracker<TModel>>
        where TModel : ObservableModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        IDependentObservablePropertyBuilder<TModel> RaiseProperty(Expression<Func<TModel, object>> expression);
    }
}