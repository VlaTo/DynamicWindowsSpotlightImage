using System;
using System.Collections;
using System.Collections.Generic;

namespace LibraProgramming.Windows.Collections
{
    public partial class SourceCollectionView
    {
        /// <summary>
        /// 
        /// </summary>
        private class EnumerableSourceIterator
        {
            private readonly Predicate<object> filter;

            public IEnumerable Source
            {
                get;
            }

            public virtual bool IsReadOnly => true;

            public virtual object this[int index]
            {
                get
                {
                    using (var enumerator = GetEnumerator())
                    {
                        enumerator.Reset();

                        while (enumerator.MoveNext() && -1 < index)
                        {
                            index--;
                        }

                        return enumerator.Current;
                    }
                }
                set
                {
//                    EnsureNotReadOnly();
                    throw new NotImplementedException();
                }
            }

            protected IRevisionProvider RevisionProvider
            {
                get;
            }

            public EnumerableSourceIterator(IRevisionProvider revisionProvider, IEnumerable source, Predicate<object> filter)
            {
                RevisionProvider = revisionProvider;
                Source = source;
                this.filter = filter;
            }

            public virtual IEnumerator<object> GetEnumerator()
            {
                return new Enumerator(RevisionProvider, Source, filter);
            }

            public virtual int GetCount()
            {
                var count = 0;

                using (var enumerator = GetEnumerator())
                {
                    enumerator.Reset();

                    while (enumerator.MoveNext())
                    {
                        count++;
                    }
                }

                return count;
            }

            public virtual int IndexOf(object item)
            {
                using (var enumerator = GetEnumerator())
                {
                    enumerator.Reset();

                    for (var index = 0; enumerator.MoveNext(); index++)
                    {
                        if (Object.Equals(item, enumerator.Current))
                        {
                            return index;
                        }
                    }
                }

                return -1;
            }

            /// <summary>
            /// 
            /// </summary>
            private class Enumerator : IEnumerator<object>
            {
                private readonly IRevisionProvider provider;
                private readonly Predicate<object> filter;
                private readonly int revision;
                private IEnumerator enumerator;
                private bool disposed;

                public object Current
                {
                    get;
                    private set;
                }

                object IEnumerator.Current => Current;

                public Enumerator(IRevisionProvider provider, IEnumerable source, Predicate<object> filter)
                {
                    this.provider = provider;
                    this.filter = filter;
                    revision = provider.Revision;
                    enumerator = source.GetEnumerator();
                }

                public bool MoveNext()
                {
                    EnsureOperable();

                    while (enumerator.MoveNext())
                    {
                        if (false == filter(enumerator.Current))
                        {
                            continue;
                        }

                        Current = enumerator.Current;

                        return true;
                    }

                    return false;
                }

                public void Reset()
                {
                    EnsureOperable();
                    enumerator.Reset();
                    Current = null;
                }

                public void Dispose()
                {
                    if (disposed)
                    {
                        return;
                    }

                    Dispose(true);
                }

                private void EnsureOperable()
                {
                    if (disposed)
                    {
                        throw new ObjectDisposedException(GetType().Name);
                    }

                    if (revision != provider.Revision)
                    {
                        throw new InvalidOperationException();
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
                            enumerator = null;
                            Current = null;
                        }
                    }
                    finally
                    {
                        disposed = true;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private class ListSourceIterator : EnumerableSourceIterator
        {
            private readonly IList list;

            public override bool IsReadOnly => list.IsReadOnly;

            public override object this[int index]
            {
                get
                {
                    return list[index];
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public ListSourceIterator(IRevisionProvider provider, IList list)
                : base(provider, list, null)
            {
                this.list = list;
            }

            public override IEnumerator<object> GetEnumerator()
            {
                return new Enumerator(RevisionProvider, list);
            }

            public override int GetCount()
            {
                return list.Count;
            }

            public override int IndexOf(object item)
            {
                return list.IndexOf(item);
            }

            /// <summary>
            /// 
            /// </summary>
            private class Enumerator : IEnumerator<object>
            {
                private readonly IRevisionProvider provider;
                private readonly Predicate<object> filter;
                private readonly int revision;
                private IEnumerator enumerator;
                private bool disposed;

                public object Current
                {
                    get;
                    private set;
                }

                object IEnumerator.Current => Current;

                public Enumerator(IRevisionProvider provider, IList list)
                {
                    this.provider = provider;
                    revision = provider.Revision;
                    enumerator = list.GetEnumerator();
                }

                public bool MoveNext()
                {
                    EnsureOperable();

                    while (enumerator.MoveNext())
                    {
                        Current = enumerator.Current;
                        return true;
                    }

                    return false;
                }

                public void Reset()
                {
                    EnsureOperable();
                    enumerator.Reset();
                    Current = null;
                }

                public void Dispose()
                {
                    if (disposed)
                    {
                        return;
                    }

                    Dispose(true);
                }

                private void EnsureOperable()
                {
                    if (disposed)
                    {
                        throw new ObjectDisposedException(GetType().Name);
                    }

                    if (revision != provider.Revision)
                    {
                        throw new InvalidOperationException();
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
                            enumerator = null;
                            Current = null;
                        }
                    }
                    finally
                    {
                        disposed = true;
                    }
                }
            }
        }
    }
}