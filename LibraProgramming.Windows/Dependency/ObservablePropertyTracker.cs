using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace LibraProgramming.Windows.Dependency
{
    public class ObservablePropertyTracker
    {
        public static IObservablePropertyTracker<TModel> Create<TModel>(Action<IObservablePropertyTrackerBuilder<TModel>> configurator)
            where TModel : ObservableModel
        {
            if (null == configurator)
            {
                throw new ArgumentNullException(nameof(configurator));
            }

            var builder = new ObservablePropertyTrackerBuilder<TModel>();

            configurator(builder);

            return builder.Construct();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    internal class ObservablePropertyTracker<TModel> : ObservablePropertyTracker, IObservablePropertyTracker<TModel>
        where TModel : ObservableModel
    {
        private IImmutableList<ObservablePropertyTrackerSubscription> subscriptions;

        public IList<ObservablePropertyDependency<TModel>> Dependencies
        {
            get;
        }

        public ObservablePropertyTracker(IList<ObservablePropertyDependency<TModel>> dependencies)
        {
            subscriptions = ImmutableList<ObservablePropertyTrackerSubscription>.Empty;
            Dependencies = dependencies;
        }

        public IObservablePropertyTrackerSubscription Subscribe(TModel model)
        {
            if (null == model)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var subscription = new ObservablePropertyTrackerSubscription(this, model);

            subscriptions = subscriptions.Add(subscription);

            return subscription;
        }

        private void ReleaseSubscription(ObservablePropertyTrackerSubscription subscription)
        {
            subscriptions = subscriptions.Remove(subscription);

            if (false == subscriptions.Any())
            {
                ;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private class ObservablePropertyTrackerSubscription : IObservablePropertyTrackerSubscription
        {
            private readonly IList<ObservablePropertyObserver> observers;
            private ObservablePropertyTracker<TModel> tracker;
            private ObservableModel model;
            private int counter;

            public ObservablePropertyTrackerSubscription(ObservablePropertyTracker<TModel> tracker, ObservableModel model)
            {
                observers = new List<ObservablePropertyObserver>();
                this.tracker = tracker;
                this.model = model;
                counter = 0;
                SubscribeToModelProperties();
            }

            private void SubscribeToModelProperties()
            {
                foreach (var dependency in tracker.Dependencies)
                {
                    var subscriptions = new List<IDisposable>();
                    var property = ObservableProperty.CreateFor(typeof(TModel), dependency.PropertyPath);

                    foreach (var observable in dependency.DependencyProperties)
                    {
                        var path = PropertyPath.Parse<TModel>(observable.Name);
                        var op = ObservableProperty.CreateFor(typeof(TModel), path);
                        var metadata = op.GetMetadata();

                        subscriptions.Add(metadata.RegisterPropertyChangedCallback(DoObservablePropertyChanged));
                    }

                    observers.Add(new ObservablePropertyObserver(property, subscriptions));
                }
            }

            public IDisposable Disable()
            {
                if (1 == Interlocked.Increment(ref counter))
                {
                    ;
                }

                return new DisposableToken(TryEnableTracking);
            }

            void IDisposable.Dispose()
            {
                if (null == tracker)
                {
                    return;
                }

                foreach (var observer in observers)
                {
                    observer.Release();
                }

                tracker.ReleaseSubscription(this);
                tracker = null;
                model = null;
            }

            private void TryEnableTracking()
            {
                if (0 == Interlocked.Decrement(ref counter))
                {
                    ;
                }
            }

            private void DoObservablePropertyChanged(ObservableModel observableModel, ObservablePropertyChangedEventArgs e)
            {
                if (0 < counter)
                {
                    return;
                }

                Debug.WriteLine("[ObservablePropertyTracker.DoObservablePropertyChanged] Property: {0}", (object) e.Property.Key.PropertyName);
            }
        }

        private class ObservablePropertyObserver
        {
            private readonly ObservableProperty property;
            private readonly IList<IDisposable> disposables;

            public ObservablePropertyObserver(ObservableProperty property, IList<IDisposable> disposables)
            {
                this.property = property;
                this.disposables = disposables;
            }

            public void Release()
            {
                while (disposables.Any())
                {
                    var index = disposables.Count - 1;

                    disposables[index].Dispose();
                    disposables.RemoveAt(index);
                }
            }
        }
    }
}