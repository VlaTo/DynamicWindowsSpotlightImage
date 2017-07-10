using System;
using System.ComponentModel;
using System.Linq;

namespace LibraProgramming.Windows.Dependency.Tracking
{
    internal class DependencySubscription<TModel> : IDependencyTracketSubscription
    {
        private DependencyTracker<TModel> tracker;
        private TModel target;
        private bool disposed;
        private int lockLevel;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:System.Object"/>.
        /// </summary>
        public DependencySubscription(DependencyTracker<TModel> tracker)
        {
            this.tracker = tracker;
        }

        public void Subscribe(TModel value)
        {
            UnhookNotification();

            target = value;

            HookNotification();
        }

        /// <summary>
        /// Выполняет определяемые приложением задачи, связанные с удалением, высвобождением или сбросом неуправляемых ресурсов.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        public IDisposable DisableTracking(bool forceSet)
        {
            return new TrackingLockToken(this, forceSet);
        }

        internal void LockTrackingInternal()
        {
            lockLevel++;
        }

        internal void UnlockTrackingInternal(bool forceSet)
        {
            if (lockLevel <= 0)
            {
                return;
            }

            lockLevel--;

            if (0 == lockLevel && forceSet)
            {
                foreach (var dependency in tracker.Dependency)
                {
                    dependency.Update(target);
                }
            }
        }

        private void UnhookNotification()
        {
            var model = target as INotifyPropertyChanged;

            if (null != model)
            {
                model.PropertyChanged -= OnPropertyChanged;
            }
        }

        private void HookNotification()
        {
            var model = target as INotifyPropertyChanged;

            if (null != model)
            {
                model.PropertyChanged += OnPropertyChanged;
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var path = PropertyPath.Parse(sender.GetType(), e.PropertyName);
            var comparer = PropertyPathComparer.Ordinal;

            foreach (var dependency in tracker.Dependency)
            {
                if (!dependency.DependencyProperties.Any(dep => comparer.Equals(path, dep)))
                {
                    continue;
                }

                dependency.Update((TModel) sender);

                return;
            }
        }

        private void Dispose(bool dispose)
        {
            if (disposed)
            {
                return;
            }

            try
            {
                if (dispose)
                {
                    UnhookNotification();
                    tracker = null;
                    target = default(TModel);
                }
            }
            finally
            {
                disposed = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private class TrackingLockToken : IDisposable
        {
            private readonly DependencySubscription<TModel> subscription;
            private readonly bool forceSet;

            public TrackingLockToken(DependencySubscription<TModel> subscription, bool forceSet)
            {
                this.subscription = subscription;
                this.forceSet = forceSet;
                subscription.LockTrackingInternal();
            }

            public void Dispose()
            {
                subscription.UnlockTrackingInternal(forceSet);
            }
        }
    }
}