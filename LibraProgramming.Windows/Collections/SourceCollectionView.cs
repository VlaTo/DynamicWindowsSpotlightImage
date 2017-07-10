using System;
using System.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using LibraProgramming.Windows.Infrastructure;

namespace LibraProgramming.Windows.Collections
{
    /// <summary>
    /// Provider extended data source with ... to data collections.
    /// </summary>
    public partial class SourceCollectionView : DependencyObject, IEnumerable, ICollectionViewFactory, IDeferrableRefresh
    {
        public static readonly DependencyProperty CanFilterProperty;
        public static readonly DependencyProperty FilterProperty;
        public static readonly DependencyProperty SourceProperty;

        private CollectionView currentView;

        /// <summary>
        /// 
        /// </summary>
        public bool CanFilter
        {
            get
            {
                return (bool) GetValue(CanFilterProperty);
            }
            set
            {
                SetValue(CanFilterProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ICollectionFilter Filter
        {
            get
            {
                return (ICollectionFilter) GetValue(FilterProperty);
            }
            set
            {
                SetValue(FilterProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable Source
        {
            get
            {
                return (IEnumerable) GetValue(SourceProperty);
            }
            set
            {
                SetValue(SourceProperty, value);
            }
        }

        int IDeferrableRefresh.DeferLevel
        {
            get;
            set;
        }

        static SourceCollectionView()
        {
            CanFilterProperty = DependencyProperty
                .Register(
                    nameof(CanFilter),
                    typeof (bool),
                    typeof (SourceCollectionView),
                    new PropertyMetadata(false, OnCanFilterPropertyChanged)
                );
            FilterProperty = DependencyProperty
                .Register(
                    nameof(Filter),
                    typeof (ICollectionFilter),
                    typeof (SourceCollectionView),
                    new PropertyMetadata(DependencyProperty.UnsetValue, OnFilterPropertyChanged)
                );
            SourceProperty = DependencyProperty
                .Register(
                    nameof(Source),
                    typeof(IEnumerable),
                    typeof(SourceCollectionView),
                    new PropertyMetadata(DependencyProperty.UnsetValue, OnSourcePropertyChanged)
                );
        }

        /// <summary>
        /// Возвращает перечислитель, который осуществляет итерацию по коллекции.
        /// </summary>
        /// <returns>
        /// Объект <see cref="T:System.Collections.IEnumerator"/>, который может использоваться для перебора коллекции.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            EnsureHasCollectionView();
            return currentView.GetEnumerator();
        }

        /// <summary>
        /// Создает экземпляр ICollectionView с использованием параметров по умолчанию.
        /// </summary>
        /// <returns>
        /// Представление по умолчанию.
        /// </returns>
        public ICollectionView CreateView()
        {
            if (null == currentView)
            {
                currentView = new CollectionView();
            }

            return currentView;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDisposable DeferRefresh()
        {
            return new RefreshDeferToken(this);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            ((IDeferrableRefresh) this).Refresh();
        }

        void IDeferrableRefresh.Refresh()
        {
            if (0 < ((IDeferrableRefresh) this).DeferLevel)
            {
                return;
            }

            if (null == currentView)
            {
                return;
            }

            using (currentView.DeferRefresh())
            {
                currentView.CollectionGroups.Clear();
                currentView.Filter = GetFilterPredicate();
                currentView.Source = Source;
            }
        }

        private void EnsureHasCollectionView()
        {
            if (null == currentView)
            {
                throw new InvalidOperationException();
            }
        }

        private Predicate<object> GetFilterPredicate()
        {
            if (CanFilter || null != Filter)
            {
                return item => Filter.CanPassFilter(this, item);
            }

            return null;
        }

        private static void OnCanFilterPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((SourceCollectionView) source).Reset();
        }

        private static void OnFilterPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((SourceCollectionView) source).Reset();
        }

        private static void OnSourcePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((SourceCollectionView) source).Reset();
        }

/*
        /// <summary>
        /// 
        /// </summary>
        private class SourceView
        {
            private readonly WeakReference source;
            private WeakReference<CollectionView> view;

            public bool HasSource => source.IsAlive;

            public CollectionView View
            {
                get
                {
                    CollectionView target;
                    return (null != view && view.TryGetTarget(out target)) ? target : null;
                }
                set
                {
                    view = new WeakReference<CollectionView>(value);
                }
            }

            public SourceView(object source)
            {
                this.source = new WeakReference(source);
            }

            public bool AreSameSource(object target)
            {
                return source.IsAlive && Object.Equals(source.Target, target);
            }
        }
*/

/*
        /// <summary>
        /// 
        /// </summary>
        private class ViewsCollection : CollectionBase
        {
            public bool TryGetView(object source, out CollectionView view)
            {
                view = null;

                for (var index = 0; index < List.Count;)
                {
                    var reference = (SourceView) List[index];

                    if (false == reference.HasSource)
                    {
                        List.RemoveAt(index);
                        continue;
                    }

                    if (reference.AreSameSource(source))
                    {
                        view = reference.View;
                        break;
                    }

                    index++;
                }

                return null != view;
            }

            public void Add(object source, CollectionView view)
            {
                for (var index = 0; index < List.Count;)
                {
                    var reference = (SourceView) List[index];

                    if (false == reference.HasSource)
                    {
                        List.RemoveAt(index);
                        continue;
                    }

                    if (reference.AreSameSource(source))
                    {
                        reference.View = view;
                        return;
                    }

                    index++;
                }

                List.Add(new SourceView(source)
                {
                    View = view
                });
            }
        }
*/
    }
}