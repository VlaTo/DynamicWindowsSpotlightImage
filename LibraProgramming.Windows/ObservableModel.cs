using System;
using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel;
using LibraProgramming.Windows.Dependency;
using LibraProgramming.Windows.Infrastructure;

namespace LibraProgramming.Windows
{
    public class ObservableModel : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private readonly ConcurrentDictionary<PropertyKey, object> properties; 
        private readonly WeakEvent<PropertyChangedEventHandler> propertyChanged;
        private readonly WeakEvent<PropertyChangingEventHandler> propertyChanging;
        private readonly object syncRoot;

        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging
        {
            add
            {
                propertyChanging.AddHandler(value);
            }
            remove
            {
                propertyChanging.RemoveHandler(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                propertyChanged.AddHandler(value);
            }
            remove
            {
                propertyChanged.RemoveHandler(value);
            }
        }

        protected ObservableModel()
        {
            propertyChanging = new WeakEvent<PropertyChangingEventHandler>();
            propertyChanged = new WeakEvent<PropertyChangedEventHandler>();
            properties = new ConcurrentDictionary<PropertyKey, object>();
            syncRoot = new object();
        }

        internal void DoPropertyChanged(PropertyChangedEventArgs e)
        {
            propertyChanged.Invoke(this, e);
        }

        protected internal object GetValue(ObservableProperty property)
        {
            if (null == property)
            {
                throw new ArgumentNullException(nameof(property));
            }

            lock (syncRoot)
            {
                var metadata = property.GetMetadata();
                object value;

                if (false == properties.TryGetValue(property.Key, out value))
                {
                    var callback = metadata.CreateDefaultValueCallback;
                    value = null == callback ? metadata.DefaultValue : callback.Invoke();
                }

                return value;
            }
        }

        protected internal void SetValue(ObservableProperty property, object value)
        {
            if (null == property)
            {
                throw new ArgumentNullException(nameof(property));
            }

            lock (syncRoot)
            {
                var metadata = property.GetMetadata();
                var key = property.Key;
                object currentValue;
                var coercedValue = metadata.CoerceValue(property, value);

                if (false == properties.TryGetValue(key, out currentValue))
                {
                    var callback = metadata.CreateDefaultValueCallback;

                    currentValue = null == callback ? metadata.DefaultValue : callback.Invoke();

                    SetValueInternal(property, currentValue, coercedValue);

                    return;
                }

                if (ReferenceEquals(currentValue, ObservableProperty.UnsetValue))
                {
                    SetValueInternal(property, currentValue, coercedValue);
                    return;
                }

                var comparer = property.Comparer;

                if (false == comparer.Equals(currentValue, coercedValue))
                {
                    SetValueInternal(property, currentValue, coercedValue);
                }
            }
        }

        protected object ReadLocalValue(ObservableProperty property)
        {
            if (null == property)
            {
                throw new ArgumentNullException(nameof(property));
            }

            lock (syncRoot)
            {
                object value;

                if (false == properties.TryGetValue(property.Key, out value))
                {
                    return null;
                }

                if (false == property.Comparer.Equals(value, ObservableProperty.UnsetValue))
                {
                    return value;
                }
            }

            return null;
        }

        private void SetValueInternal(ObservableProperty property, object oldValue, object newValue)
        {
            var metadata = property.GetMetadata();
            var key = property.Key;

            try
            {
                propertyChanging.Invoke(this, new PropertyChangingEventArgs(key.PropertyName));

                properties[key] = newValue;
            }
            finally
            {
                metadata.RaisePropertyChanged(this, property, oldValue, newValue);
            }
        }
    }
}