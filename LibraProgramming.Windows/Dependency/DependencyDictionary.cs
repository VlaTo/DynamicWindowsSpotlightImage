using System.Collections.Generic;
using LibraProgramming.Windows.Dependency.Tracking;

namespace LibraProgramming.Windows.Dependency
{
    internal sealed class DependencyDictionary<TModel> : Dictionary<PropertyPath, PropertyDependency<TModel>>
    {
        /// <summary>
        /// Инициализирует новый пустой экземпляр класса <see cref="T:System.Collections.Generic.Dictionary`2"/>, имеющий начальную
        /// емкость по умолчанию и использующий функцию сравнения по умолчанию, проверяющую равенство для данного типа ключа.
        /// </summary>
        public DependencyDictionary()
        {
        }

        /// <summary>
        /// Инициализирует новый пустой экземпляр класса <see cref="T:System.Collections.Generic.Dictionary`2"/> начальной емкостью по умолчанию,
        /// использующий указанную функцию сравнения <see cref="T:System.Collections.Generic.IEqualityComparer`1"/>.
        /// </summary>
        /// <param name="comparer">Реализация <see cref="T:System.Collections.Generic.IEqualityComparer`1"/>,
        /// которую следует использовать при сравнении ключей, или null, если для данного типа ключа должна использоваться
        /// реализация <see cref="T:System.Collections.Generic.EqualityComparer`1"/> по умолчанию.
        /// </param>
        public DependencyDictionary(IEqualityComparer<PropertyPath> comparer)
            : base(comparer)
        {
        }
    }
}