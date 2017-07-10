using System;
using System.Collections.Generic;

namespace LibraProgramming.Windows.Dependency.Tracking
{
    internal class PropertyDependency<TModel>
    {
        public IList<PropertyPath> DependencyProperties
        {
            get;
        }

        public PropertyPath PropertyPath
        {
            get;
        }

        public Action<TModel> Calculator
        {
            get;
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:System.Object"/>.
        /// </summary>
        public PropertyDependency(PropertyPath path, Action<TModel> calculator)
        {
            PropertyPath = path;
            Calculator = calculator;
            DependencyProperties = new List<PropertyPath>();
        }

        public void Update(TModel model)
        {
            Calculator.Invoke(model);
        }
    }
}