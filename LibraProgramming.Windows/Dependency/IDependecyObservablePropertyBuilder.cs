using System;
using System.Linq.Expressions;

namespace LibraProgramming.Windows.Dependency
{
    public interface IDependecyObservablePropertyBuilder<TModel>
        where TModel : ObservableModel
    {
        IDependecyObservablePropertyBuilder<TModel> WhenChanged(Expression<Func<TModel, object>> selector);
    }
}