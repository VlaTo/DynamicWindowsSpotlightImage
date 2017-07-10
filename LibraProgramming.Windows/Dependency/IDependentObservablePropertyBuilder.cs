using System;

namespace LibraProgramming.Windows.Dependency
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IDependentObservablePropertyBuilder<TModel>
        where TModel : ObservableModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="calculator"></param>
        /// <returns></returns>
        IDependecyObservablePropertyBuilder<TModel> CalculatedBy(Func<TModel, object> calculator);
    }
}