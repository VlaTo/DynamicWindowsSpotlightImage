using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;

namespace LibraProgramming.Windows.Dependency
{
    /// <summary>
    /// 
    /// </summary>
    public class ObservablePropertyChangedEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public ObservableProperty Property
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        public object NewValue
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        public object OldValue
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="newValue"></param>
        /// <param name="oldValue"></param>
        internal ObservablePropertyChangedEventArgs(ObservableProperty property, object newValue, object oldValue)
        {
            Property = property;
            NewValue = newValue;
            OldValue = oldValue;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <param name="e"></param>
    public delegate void ObservablePropertyChangedCallback(ObservableModel model, ObservablePropertyChangedEventArgs e);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public delegate object CreateDefaultValueCallback();

    /// <summary>
    /// 
    /// </summary>
    public class ObservablePropertyMetadata
    {
        private readonly ObservablePropertyChangedCallback callback;
        private ImmutableList<ObservablePropertyChangedCallback> callbacks;

        /// <summary>
        /// 
        /// </summary>
        public CreateDefaultValueCallback CreateDefaultValueCallback
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        public object DefaultValue
        {
            get;
        }

        public ObservablePropertyMetadata(object defaultValue)
            : this(defaultValue, null, null)
        {
        }

        public ObservablePropertyMetadata(object defaultValue, ObservablePropertyChangedCallback propertyChangedCallback)
            : this(defaultValue, null, propertyChangedCallback)
        {
        }

        public ObservablePropertyMetadata(CreateDefaultValueCallback createDefaultValueCallback)
            : this(ObservableProperty.UnsetValue, createDefaultValueCallback, null)
        {
        }

        public ObservablePropertyMetadata(CreateDefaultValueCallback createDefaultValueCallback,
            ObservablePropertyChangedCallback propertyChangedCallback)
            : this(ObservableProperty.UnsetValue, createDefaultValueCallback, propertyChangedCallback)
        {
        }

        protected ObservablePropertyMetadata(
            object defaultValue,
            CreateDefaultValueCallback createDefaultValueCallback,
            ObservablePropertyChangedCallback propertyChangedCallback)
        {
            callbacks = ImmutableList<ObservablePropertyChangedCallback>.Empty;
            callback = propertyChangedCallback;
            DefaultValue = defaultValue;
            CreateDefaultValueCallback = createDefaultValueCallback;
        }

        protected internal virtual object CoerceValue(ObservableProperty property, object value)
        {
            return value;
        }

        protected internal virtual void RaisePropertyChanged(ObservableModel model, ObservableProperty property,
            object oldValue,
            object newValue)
        {
            var args = new ObservablePropertyChangedEventArgs(property, newValue, oldValue);

            try
            {
                callback?.Invoke(model, args);

                foreach (var action in callbacks)
                {
                    action.Invoke(model, args);
                }
            }
            finally
            {
                model.DoPropertyChanged(new PropertyChangedEventArgs(property.Key.PropertyName));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static ObservablePropertyMetadata Create(object defaultValue)
        {
            return new ObservablePropertyMetadata(defaultValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <param name="propertyChangedCallback"></param>
        /// <returns></returns>
        public static ObservablePropertyMetadata Create(object defaultValue,
            ObservablePropertyChangedCallback propertyChangedCallback)
        {
            return new ObservablePropertyMetadata(defaultValue, propertyChangedCallback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createDefaultValueCallback"></param>
        /// <returns></returns>
        public static ObservablePropertyMetadata Create(CreateDefaultValueCallback createDefaultValueCallback)
        {
            return new ObservablePropertyMetadata(createDefaultValueCallback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createDefaultValueCallback"></param>
        /// <param name="propertyChangedCallback"></param>
        /// <returns></returns>
        public static ObservablePropertyMetadata Create(CreateDefaultValueCallback createDefaultValueCallback,
            ObservablePropertyChangedCallback propertyChangedCallback)
        {
            return new ObservablePropertyMetadata(createDefaultValueCallback, propertyChangedCallback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public IDisposable RegisterPropertyChangedCallback(ObservablePropertyChangedCallback action)
        {
            if (null == action)
            {
                throw new ArgumentNullException(nameof(action));
            }

            callbacks = callbacks.Add(action);

            return new DisposableToken<ObservablePropertyChangedCallback>(action, ReleasePropertyChangedCallback);
        }

        private void ReleasePropertyChangedCallback(ObservablePropertyChangedCallback action)
        {
            callbacks = callbacks.Remove(action);
        }
    }
}