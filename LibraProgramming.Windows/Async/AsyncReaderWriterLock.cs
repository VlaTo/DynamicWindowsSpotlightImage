using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using LibraProgramming.Windows.Infrastructure;

namespace LibraProgramming.Windows.Async
{
    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("Id = {Id}, State = {StateForDebugger}")]
    [DebuggerTypeProxy(typeof(DebugView))]
    public sealed class AsyncReaderWriterLock : IReaderWriterLock
    {
        private int id;
        private int locksHeld;
        private readonly IAsyncWaitQueue<IDisposable> writers;
        private readonly IAsyncWaitQueue<IDisposable> readers;
        private readonly IAsyncWaitQueue<UpgradeableReaderKey> upgradeableReaders;
        private readonly IAsyncWaitQueue<IDisposable> upgradeReaders;
        private UpgradeableReaderKey upgradeableReaderKey;
        private readonly Task<IDisposable> cachedReaderKey;
        private readonly Task<IDisposable> cachedWriterKey;

        /// <summary>
        /// 
        /// </summary>
        public int Id => IdManager<AsyncReaderWriterLock>.GetId(ref id);

        /// <summary>
        /// 
        /// </summary>
        internal object SyncRoot
        {
            get;
        }

        [DebuggerNonUserCode]
        internal State StateForDebugger
        {
            get
            {
                if (0 == locksHeld)
                {
                    return State.Unlocked;
                }

                if (-1 == locksHeld)
                {
                    return null == upgradeableReaderKey ? State.WriteLocked : State.WriteLockedWithUpgredableReader;
                }

                if (null != upgradeableReaderKey)
                {
                    return State.ReadLockedWithUpgredableReader;
                }

                return State.ReadLocked;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public AsyncReaderWriterLock()
            : this(
                new AsyncWaitQueue<IDisposable>(),
                new AsyncWaitQueue<IDisposable>(),
                new AsyncWaitQueue<UpgradeableReaderKey>(),
                new AsyncWaitQueue<IDisposable>())
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writers"></param>
        /// <param name="readers"></param>
        /// <param name="upgradeableReaders"></param>
        /// <param name="upgradeReaders"></param>
        public AsyncReaderWriterLock(
            IAsyncWaitQueue<IDisposable> writers,
            IAsyncWaitQueue<IDisposable> readers,
            IAsyncWaitQueue<UpgradeableReaderKey> upgradeableReaders, 
            IAsyncWaitQueue<IDisposable> upgradeReaders)
        {
            SyncRoot = new object();
            cachedReaderKey = Task.FromResult<IDisposable>(new ReaderKey(this));
            cachedWriterKey = Task.FromResult<IDisposable>(new WriterKey(this));
            this.writers = writers;
            this.readers = readers;
            this.upgradeableReaders = upgradeableReaders;
            this.upgradeReaders = upgradeReaders;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public AwaitableDisposable<IDisposable> AccquireReaderLockAsync(CancellationToken ct)
        {
            Task<IDisposable> task;

            lock (SyncRoot)
            {
                if (locksHeld >= 0 && writers.IsEmpty && upgradeableReaders.IsEmpty && upgradeReaders.IsEmpty)
                {
                    locksHeld++;
                    task = cachedReaderKey;
                }
                else
                {
                    task = readers.Enqueue(SyncRoot, ct);
                }
            }

            return new AwaitableDisposable<IDisposable>(task);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public IDisposable AccquireReaderLock(CancellationToken ct)
        {
            Task<IDisposable> task;

            lock (SyncRoot)
            {
                if (locksHeld >= 0 && writers.IsEmpty && upgradeableReaders.IsEmpty && upgradeReaders.IsEmpty)
                {
                    locksHeld++;
                    return cachedReaderKey.Result;
                }

                task = readers.Enqueue(SyncRoot, ct);
            }

            return task.WaitAndUnwrapException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public AwaitableDisposable<IDisposable> AccquireReaderLockAsync()
        {
            return AccquireReaderLockAsync(CancellationToken.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDisposable AccquireReaderLock()
        {
            return AccquireReaderLock(CancellationToken.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public AwaitableDisposable<IDisposable> AccquireWriterLockAsync(CancellationToken ct)
        {
            Task<IDisposable> task;

            lock (SyncRoot)
            {
                if (0 == locksHeld)
                {
                    locksHeld = -1;
                    task = cachedWriterKey;
                }
                else
                {
                    task = writers.Enqueue(SyncRoot, ct);
                }
            }

            ReleaseWaitersWhenCanceled(task);

            return new AwaitableDisposable<IDisposable>(task);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public IDisposable AccquireWriterLock(CancellationToken ct)
        {
            Task<IDisposable> task;

            lock (SyncRoot)
            {
                if (0 == locksHeld)
                {
                    locksHeld = -1;
                    return cachedWriterKey.Result;
                }

                task = writers.Enqueue(SyncRoot, ct);
            }

            ReleaseWaitersWhenCanceled(task);

            return task.WaitAndUnwrapException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public AwaitableDisposable<IDisposable> AccquireWriterLockAsync()
        {
            return AccquireWriterLockAsync(CancellationToken.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDisposable AccquireWriterLock()
        {
            return AccquireWriterLock(CancellationToken.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public AwaitableDisposable<UpgradeableReaderKey> AccquireUpgradeableReaderLockAsync(CancellationToken ct)
        {
            Task<UpgradeableReaderKey> task;

            lock (SyncRoot)
            {
                if (0 == locksHeld || (locksHeld > 0 && null == upgradeableReaderKey))
                {
                    locksHeld++;
                    upgradeableReaderKey = new UpgradeableReaderKey(this);
                    task = Task.FromResult(upgradeableReaderKey);
                }
                else
                {
                    task = upgradeableReaders.Enqueue(SyncRoot, ct);
                }
            }

            ReleaseWaitersWhenCanceled(task);

            return new AwaitableDisposable<UpgradeableReaderKey>(task);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public UpgradeableReaderKey AccquireUpgradeableReaderLock(CancellationToken ct)
        {
            Task<UpgradeableReaderKey> task;

            lock (SyncRoot)
            {
                if (0 == locksHeld || (locksHeld > 0 && null == upgradeableReaderKey))
                {
                    locksHeld++;
                    upgradeableReaderKey = new UpgradeableReaderKey(this);
                    return upgradeableReaderKey;
                }

                task = upgradeableReaders.Enqueue(SyncRoot, ct);
            }

            ReleaseWaitersWhenCanceled(task);

            return task.WaitAndUnwrapException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public AwaitableDisposable<UpgradeableReaderKey> AccquireUpgradeableReaderLockAsync()
        {
            return AccquireUpgradeableReaderLockAsync(CancellationToken.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public UpgradeableReaderKey AccquireUpgradeableReaderLock()
        {
            return AccquireUpgradeableReaderLock(CancellationToken.None);
        }

        internal Task<IDisposable> UpgradeAsync(CancellationToken ct)
        {
            Task<IDisposable> task;

            if (1 == locksHeld)
            {
                locksHeld = -1;
                task = upgradeableReaderKey.cachedUpgradeKey;
            }
            else
            {
                task = upgradeReaders.Enqueue(SyncRoot, ct);
            }

            return task;
        }

        internal IReadOnlyCollection<IDisposable> Downgrade()
        {
            locksHeld = 1;
            return ReleaseWaiters();
        }

        internal void ReleaseReaderLock()
        {
            IEnumerable<IDisposable> finalizers;

            lock (SyncRoot)
            {
                locksHeld--;
                finalizers = ReleaseWaiters();
            }

            foreach (var finalizer in finalizers)
            {
                finalizer.Dispose();
            }
        }

        internal void ReleaseWriterLock()
        {
            IEnumerable<IDisposable> finalizers;

            lock (SyncRoot)
            {
                locksHeld = 0;
                finalizers = ReleaseWaiters();
            }

            foreach (var finalizer in finalizers)
            {
                finalizer.Dispose();
            }
        }

        internal void ReleaseUpgradeableReaderLock(Task upgrade)
        {
            IDisposable cancelFinish = null;
            IEnumerable<IDisposable> finalizers;

            lock (SyncRoot)
            {
                if (null != upgrade)
                {
                    cancelFinish = upgradeReaders.TryCancel(upgrade);
                }

                upgradeableReaderKey = null;
                locksHeld--;
                finalizers = ReleaseWaiters();
            }

            cancelFinish?.Dispose();

            foreach (var finalizer in finalizers)
            {
                finalizer.Dispose();
            }
        }

        private IReadOnlyCollection<IDisposable> ReleaseWaiters()
        {
            var waiters = new List<IDisposable>();

            if (0 == locksHeld)
            {
                if (false == writers.IsEmpty)
                {
                    waiters.Add(writers.Dequeue(cachedWriterKey.Result));
                    locksHeld = -1;

                    return waiters;
                }

                if (false == upgradeableReaders.IsEmpty)
                {
                    upgradeableReaderKey = new UpgradeableReaderKey(this);
                    waiters.Add(upgradeableReaders.Dequeue(upgradeableReaderKey));
                    locksHeld++;
                }

                while (false == readers.IsEmpty)
                {
                    waiters.Add(readers.Dequeue(cachedReaderKey.Result));
                }

                return waiters;
            }

            if (1 == locksHeld)
            {
                if (false == upgradeReaders.IsEmpty)
                {
                    waiters.Add(upgradeReaders.Dequeue(upgradeableReaderKey.cachedUpgradeKey.Result));
                    locksHeld = -1;
                }
            }

            if (0 < locksHeld)
            {
                if (false == writers.IsEmpty || false == upgradeableReaders.IsEmpty || false == upgradeReaders.IsEmpty)
                {
                    return waiters;
                }

                if (null == upgradeableReaderKey && false == upgradeableReaders.IsEmpty)
                {
                    upgradeableReaderKey = new UpgradeableReaderKey(this);
                    waiters.Add(upgradeableReaders.Dequeue(upgradeableReaderKey));
                }
            }

            return waiters;
        }

        private void ReleaseWaitersWhenCanceled(Task task)
        {
            task.ContinueWith(t =>
            {
                IEnumerable<IDisposable> finalizers;

                lock (SyncRoot)
                {
                    finalizers = ReleaseWaiters();
                }

                foreach (var finalizer in finalizers)
                {
                    finalizer.Dispose();
                }
            }, CancellationToken.None,
                TaskContinuationOptions.OnlyOnCanceled | TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);
        }

        private sealed class ReaderKey : IDisposable
        {
            private readonly AsyncReaderWriterLock @lock;

            public ReaderKey(AsyncReaderWriterLock @lock)
            {
                this.@lock = @lock;
            }

            public void Dispose()
            {
                @lock.ReleaseReaderLock();
            }
        }

        private sealed class WriterKey : IDisposable
        {
            private readonly AsyncReaderWriterLock @lock;

            public WriterKey(AsyncReaderWriterLock @lock)
            {
                this.@lock = @lock;
            }

            public void Dispose()
            {
                @lock.ReleaseWriterLock();
            }
        }

        [DebuggerDisplay("State = {StateForDebugger}, ReaderWriterLockId = {@lock.Id}")]
        public sealed class UpgradeableReaderKey : IDisposable
        {
            private readonly AsyncReaderWriterLock @lock;
            private Task<IDisposable> upgrade;
            internal readonly Task<IDisposable> cachedUpgradeKey;

            public bool Upgraded
            {
                get
                {
                    Task task;

                    lock (@lock.SyncRoot)
                    {
                        task = upgrade;
                    }

                    return null != task && TaskStatus.RanToCompletion == task.Status;
                }
            }

            [DebuggerNonUserCode]
            internal State StateForDebugger
            {
                get
                {
                    if (null == upgrade)
                    {
                        return State.Reader;
                    }

                    if (TaskStatus.RanToCompletion == upgrade.Status)
                    {
                        return State.Writer;
                    }

                    return State.UpgradingToWriter;
                }
            }

            internal UpgradeableReaderKey(AsyncReaderWriterLock @lock)
            {
                this.@lock = @lock;
                cachedUpgradeKey = Task.FromResult<IDisposable>(new UpgradeKey(this));
            }

            public AwaitableDisposable<IDisposable> UpgradeAsync(CancellationToken ct)
            {
                lock (@lock.SyncRoot)
                {
                    if (null != upgrade)
                    {
                        throw new InvalidOperationException();
                    }

                    upgrade = @lock.UpgradeAsync(ct);
                }

                @lock.ReleaseWaitersWhenCanceled(upgrade);

                var tcs = new TaskCompletionSource<IDisposable>();

                upgrade.ContinueWith(task =>
                {
                    if (task.IsCanceled)
                    {
                        lock (@lock.SyncRoot)
                        {
                            upgrade = null;
                        }

                        tcs.TryCompleteFromTask(task);
                    }
                }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

                return new AwaitableDisposable<IDisposable>(tcs.Task);
            }

            public IDisposable Upgrade(CancellationToken ct)
            {
                lock (@lock.SyncRoot)
                {
                    if (null != upgrade)
                    {
                        throw new InvalidOperationException();
                    }

                    upgrade = @lock.UpgradeAsync(ct);
                }

                @lock.ReleaseWaitersWhenCanceled(upgrade);

                var tcs = new TaskCompletionSource<IDisposable>();

                upgrade.ContinueWith(task =>
                {
                    if (task.IsCanceled)
                    {
                        lock (@lock.SyncRoot)
                        {
                            upgrade = null;
                        }
                    }

                    tcs.TryCompleteFromTask(task);
                }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

                return tcs.Task.WaitAndUnwrapException();
            }

            public AwaitableDisposable<IDisposable> UpgradeAsync()
            {
                return UpgradeAsync(CancellationToken.None);
            }

            public IDisposable Upgrade()
            {
                return Upgrade(CancellationToken.None);
            }

            public void Dispose()
            {
                @lock.ReleaseUpgradeableReaderLock(upgrade);
            }

            private void Downgrade()
            {
                IEnumerable<IDisposable> finalizers;

                lock (@lock.SyncRoot)
                {
                    finalizers = @lock.Downgrade();
                    upgrade = null;
                }

                foreach (var finalizer in finalizers)
                {
                    finalizer.Dispose();
                }
            }

            internal sealed class UpgradeKey : IDisposable
            {
                private readonly UpgradeableReaderKey key;

                public UpgradeKey(UpgradeableReaderKey key)
                {
                    this.key = key;
                }

                public void Dispose()
                {
                    key.Downgrade();
                }
            }

            internal enum State
            {
                Reader,
                Writer,
                UpgradingToWriter
            }
        }

        internal enum State
        {
            Unlocked,
            ReadLocked,
            ReadLockedWithUpgredableReader,
            WriteLocked,
            WriteLockedWithUpgredableReader
        }

        [DebuggerNonUserCode]
        // ReSharper disable UnusedMember.Local
        private sealed class DebugView
        {
            private readonly AsyncReaderWriterLock @lock;

            public int Id => @lock.Id;

            public State State => @lock.StateForDebugger;

            public IAsyncWaitQueue<IDisposable> ReaderWaitQueue => @lock.readers;

            public IAsyncWaitQueue<IDisposable> WriterWaitQueue => @lock.writers;

            /// <summary>
            /// Инициализирует новый экземпляр класса <see cref="T:System.Object"/>.
            /// </summary>
            public DebugView(AsyncReaderWriterLock @lock)
            {
                this.@lock = @lock;
            }
        }
        // ReSharper restore UnusedMember.Local
    }
}