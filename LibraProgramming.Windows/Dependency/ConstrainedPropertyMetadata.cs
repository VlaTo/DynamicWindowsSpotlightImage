using System;

namespace LibraProgramming.Windows.Dependency
{
    /// <summary>
    /// 
    /// </summary>
    public class CoerceValueEventArgs
    {
        public object SourceValue
        {
            get;
        }

        public object CoersedValue
        {
            get;
            set;
        }

        public CoerceValueEventArgs(object sourceValue)
        {
            SourceValue = sourceValue;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="property"></param>
    /// <param name="e"></param>
    public delegate void CoerceValueCallback(ObservableProperty property, CoerceValueEventArgs e);

    /// <summary>
    /// 
    /// </summary>
    public class ConstrainedPropertyMetadata : ObservablePropertyMetadata
    {
        public CoerceValueCallback CoerceValueCallback
        {
            get;
        }

        public ConstrainedPropertyMetadata(Object defaultValue, CoerceValueCallback coerceValueCallback)
            : base(defaultValue)
        {
            if (null == coerceValueCallback)
            {
                throw new ArgumentNullException(nameof(coerceValueCallback));
            }

            CoerceValueCallback = coerceValueCallback;
        }

        public ConstrainedPropertyMetadata(
            Object defaultValue, 
            ObservablePropertyChangedCallback propertyChangedCallback,
            CoerceValueCallback coerceValueCallback)
            : base(defaultValue, propertyChangedCallback)
        {
            if (null == coerceValueCallback)
            {
                throw new ArgumentNullException(nameof(coerceValueCallback));
            }

            CoerceValueCallback = coerceValueCallback;
        }

        public ConstrainedPropertyMetadata(
            CreateDefaultValueCallback createDefaultValueCallback,
            CoerceValueCallback coerceValueCallback)
            : base(createDefaultValueCallback)
        {
            if (null == coerceValueCallback)
            {
                throw new ArgumentNullException(nameof(coerceValueCallback));
            }

            CoerceValueCallback = coerceValueCallback;
        }

        public ConstrainedPropertyMetadata(
            CreateDefaultValueCallback createDefaultValueCallback,
            ObservablePropertyChangedCallback propertyChangedCallback,
            CoerceValueCallback coerceValueCallback)
            : base(createDefaultValueCallback, propertyChangedCallback)
        {
            if (null == coerceValueCallback)
            {
                throw new ArgumentNullException(nameof(coerceValueCallback));
            }

            CoerceValueCallback = coerceValueCallback;
        }

        public ConstrainedPropertyMetadata(
            Object defaultValue,
            CreateDefaultValueCallback createDefaultValueCallback,
            ObservablePropertyChangedCallback propertyChangedCallback,
            CoerceValueCallback coerceValueCallback)
            : base(defaultValue, createDefaultValueCallback, propertyChangedCallback)
        {
            if (null == coerceValueCallback)
            {
                throw new ArgumentNullException(nameof(coerceValueCallback));
            }

            CoerceValueCallback = coerceValueCallback;
        }

        protected internal override object CoerceValue(ObservableProperty property, object value)
        {
            var e = new CoerceValueEventArgs(value);

            CoerceValueCallback.Invoke(property, e);

            return e.CoersedValue;
        }
    }
}