using System;
using System.Linq.Expressions;

namespace LibraProgramming.Windows.Dependency.Tracking
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IDependencyPropertySelector<TModel>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        IDependencyPropertySelector<TModel> WhenPropertyChanged(Expression<Func<TModel, object>> expression);
    }
}