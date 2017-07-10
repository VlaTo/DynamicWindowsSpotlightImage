using System;
using System.Collections;
using System.Collections.Generic;

namespace LibraProgramming.Windows.Collections
{
    public partial class SourceCollectionView
    {
/*
        /// <summary>
        /// 
        /// </summary>
        private class PlainEnumerator : IEnumerator<object>
        {
            private IEnumerator enumerator;
            private bool disposed;

            public object Current
            {
                get
                {
                    EnsureNotDisposed();
                    return enumerator.Current;
                }
            }

            object IEnumerator.Current => Current;

            public PlainEnumerator(IEnumerable source)
            {
                if (null == source)
                {
                    throw new ArgumentNullException(nameof(source));
                }

                enumerator = source.GetEnumerator();
            }

            public virtual bool MoveNext()
            {
                EnsureNotDisposed();
                return enumerator.MoveNext();
            }

            public virtual void Reset()
            {
                EnsureNotDisposed();
                enumerator.Reset();
            }

            public void Dispose()
            {
                if (false == disposed)
                {
                    Dispose(true);
                }
            }

            protected virtual void DisposeOverride()
            {
                enumerator = null;
            }

            private void EnsureNotDisposed()
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
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
                        DisposeOverride();
                    }
                }
                finally
                {
                    disposed = true;
                }
            }
        }
*/
    }
}