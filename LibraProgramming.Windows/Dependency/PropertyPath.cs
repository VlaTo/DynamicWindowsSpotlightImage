using System;
using System.Linq.Expressions;
using System.Reflection;

namespace LibraProgramming.Windows.Dependency
{
    internal class PropertyPath : IEquatable<PropertyPath>
    {
        private readonly MemberInfo member;

        public string Name => member.Name;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:System.Object"/>.
        /// </summary>
        private PropertyPath(MemberInfo member)
        {
            this.member = member;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static PropertyPath Parse(Expression expression)
        {
            while (true)
            {
                var lambda = expression as LambdaExpression;

                if (null != lambda)
                {
                    expression = lambda.Body;
                    continue;
                }

                var access = expression as MemberExpression;

                if (null != access)
                {
                    return new PropertyPath(access.Member);
                }

                var unary = expression as UnaryExpression;

                if (null == unary)
                {
                    throw new NotSupportedException();
                }

                if (ExpressionType.Convert == unary.NodeType)
                {
                    expression = unary.Operand;
                    continue;
                }

                break;
            }

            throw new NotSupportedException();
        }

        public static PropertyPath Parse<TObject>(string name)
        {
            if (null == name)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return Parse(typeof (TObject), name);
        }

        public static PropertyPath Parse(Type type, string name)
        {
            if (null == type)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (null == name)
            {
                throw new ArgumentNullException(nameof(name));
            }

            while (true)
            {
                var member = type.GetRuntimeProperty(name);
                return new PropertyPath(member);
            }
        }

        /// <summary>
        /// Указывает, равен ли текущий объект другому объекту того же типа.
        /// </summary>
        /// <returns>
        /// true, если текущий объект равен параметру <paramref name="other"/>, в противном случае — false.
        /// </returns>
        /// <param name="other">Объект, который требуется сравнить с данным объектом.</param>
        public bool Equals(PropertyPath other)
        {
            if (null == other)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return member.Equals(other.member);
        }

        /// <summary>
        /// Определяет, равен ли заданный объект текущему объекту.
        /// </summary>
        /// <returns>
        /// true, если заданный объект равен текущему объекту; в противном случае — false.
        /// </returns>
        /// <param name="obj">Объект, который требуется сравнить с текущим объектом. </param>
        public override bool Equals(object obj)
        {
            var path = obj as PropertyPath;

            if (null != path)
            {
                return Equals(path);
            }

            return base.Equals(obj);
        }
    }
}