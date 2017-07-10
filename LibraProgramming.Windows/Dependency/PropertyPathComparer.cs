using System.Collections.Generic;

namespace LibraProgramming.Windows.Dependency
{
    internal class PropertyPathComparer : IEqualityComparer<PropertyPath>
    {
        internal static IEqualityComparer<PropertyPath> Ordinal
        {
            get;
        }

        static PropertyPathComparer()
        {
            Ordinal = new PropertyPathComparer();
        }

        /// <summary>
        /// Определяет, равны ли два указанных объекта.
        /// </summary>
        /// <returns>
        /// true, если указанные объекты равны; в противном случае — false.
        /// </returns>
        /// <param name="x">Первый сравниваемый объект типа <paramref name="T"/>.</param>
        /// <param name="y">Второй сравниваемый объект типа <paramref name="T"/>.</param>
        public bool Equals(PropertyPath x, PropertyPath y)
        {
            if (ReferenceEquals(null, x))
            {
                return false;
            }

            if (ReferenceEquals(null, y))
            {
                return false;
            }

            if (ReferenceEquals(x, y))
            {
                return true;
            }

            return x.Equals(y);
        }

        /// <summary>
        /// Возвращает хэш-код указанного объекта.
        /// </summary>
        /// <returns>
        /// Хэш-код указанного объекта.
        /// </returns>
        /// <param name="obj">Объект <see cref="T:System.Object"/>, для которого необходимо возвратить хэш-код.</param><exception cref="T:System.ArgumentNullException">Тип параметра <paramref name="obj"/> является ссылочным типом и значение параметра <paramref name="obj"/> — null.</exception>
        public int GetHashCode(PropertyPath obj)
        {
            return null != obj ? obj.GetHashCode() : 0;
        }
    }
}