using System;

namespace LibraProgramming.Windows.Dependency
{
    /// <summary>
    /// 
    /// </summary>
    internal class PropertyKey : IEquatable<PropertyKey>
    {
        public string PropertyName
        {
            get;
        }

        public Type PropertyType
        {
            get;
        }

        public PropertyKey(string propertyName, Type propertyType)
        {
            PropertyName = propertyName;
            PropertyType = propertyType;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (PropertyName.GetHashCode() * 397) ^ PropertyType.GetHashCode();
            }
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            var key = other as PropertyKey;

            if (ReferenceEquals(null, key))
            {
                return false;
            }

            return Equals(key);
        }

        public bool Equals(PropertyKey other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return String.Equals(PropertyName, other.PropertyName, StringComparison.Ordinal) && PropertyType.Equals(other.PropertyType);
        }

        /*public static bool operator ==(PropertyKey left, PropertyKey right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(PropertyKey left, PropertyKey right)
            {
                return !Equals(left, right);
            }*/
    }
}