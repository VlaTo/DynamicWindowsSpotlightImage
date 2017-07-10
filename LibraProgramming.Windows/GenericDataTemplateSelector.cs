using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using LibraProgramming.Windows.Infrastructure;

namespace LibraProgramming.Windows
{
    /// <summary>
    /// Generic Data Template selector class.
    /// </summary>
    public class GenericDataTemplateSelector : DataTemplateSelector
    {
        private ResourceDictionary resources;
        private readonly WeakEventHandler resourcesChanged;

        /// <summary>
        /// Gets or sets <see cref="ResourceDictionary" /> to work with.
        /// </summary>
        public ResourceDictionary Resources
        {
            get
            {
                return resources;
            }
            set
            {
                if (ReferenceEquals(resources, value))
                {
                    return;
                }

                resources = value;

                DoResourcesChanged();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler ResourcesChanged
        {
            add
            {
                resourcesChanged.AddHandler(value);
            }
            remove
            {
                resourcesChanged.RemoveHandler(value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the DataTemplateSelector class.
        /// </summary>
        public GenericDataTemplateSelector()
        {
            resourcesChanged = new WeakEventHandler();
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (null == item)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var key = item.GetType().FullName;
            DataTemplate candidate;

            if (null != resources && TryGetResource(resources, key, out candidate))
            {
                return candidate;
            }

            return TryGetResource(Application.Current.Resources, key, out candidate)
                ? candidate
                : base.SelectTemplateCore(item, container);
        }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (null == item)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var key = item.GetType().FullName;
            DataTemplate candidate;

            if (null != resources && TryGetResource(resources, key, out candidate))
            {
                return candidate;
            }

            return TryGetResource(Application.Current.Resources, key, out candidate)
                ? candidate
                : base.SelectTemplateCore(item);
        }

        private bool TryGetResource(ResourceDictionary rd, string key, out DataTemplate template)
        {
            if (null == rd)
            {
                throw new ArgumentNullException(nameof(rd));
            }

            if (null == key)
            {
                throw new ArgumentNullException(nameof(key));
            }

            template = rd.ContainsKey(key) ? rd[key] as DataTemplate : null;

            return null != template;
        }

        private void DoResourcesChanged()
        {
            resourcesChanged.Invoke(this, EventArgs.Empty);
        }
    }
}