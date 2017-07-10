using System;
using System.Collections.Generic;

namespace LibraProgramming.Windows.Dependency
{
    internal class ObservablePropertyDependency<TModel> : IObservablePropertyDependency<TModel>
        where TModel : ObservableModel
    {
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
        public PropertyPath PropertyPath
        {
            get;
        }

        public ObservablePropertyDependency(PropertyPath propertyPath, Func<TModel, object> calculator)
        {
            PropertyPath = propertyPath;
            DependencyProperties = new HashSet<PropertyPath>(PropertyPathComparer.Ordinal);
        }
    }
}