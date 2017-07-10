using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Windows.Foundation.Collections;

namespace LibraProgramming.Windows.Collections
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    public class ObservableVector<TObject> : CollectionBase, IObservableVector<TObject>
    {
        private long revision;

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly => InnerList.IsReadOnly;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TObject this[int index]
        {
            get
            {
                return (TObject) InnerList[index];
            }
            set
            {
                InnerList[index] = value;
                DoCollectionChanged(CollectionChangedEventArgs.ItemChanged(index));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public event VectorChangedEventHandler<TObject> VectorChanged;
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator<TObject> IEnumerable<TObject>.GetEnumerator()
        {
            return new Enumerator(this, revision);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(TObject item)
        {
            var index = InnerList.Add(item);
            DoCollectionChanged(CollectionChangedEventArgs.ItemAdded(index));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(TObject item)
        {
            return InnerList.Contains(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(TObject[] array, int arrayIndex)
        {
            InnerList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(TObject item)
        {
            var index = InnerList.IndexOf(item);

            if (-1 < index)
            {
                InnerList.RemoveAt(index);
                DoCollectionChanged(CollectionChangedEventArgs.ItemRemoved(index));
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(TObject item)
        {
            return InnerList.IndexOf(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, TObject item)
        {
            InnerList.Insert(index, item);
            DoCollectionChanged(CollectionChangedEventArgs.ItemInserted(index));
        }

        private void DoCollectionChanged(CollectionChangedEventArgs e)
        {
            revision++;
            VectorChanged?.Invoke(this, e);
        }

        /// <summary>
        /// 
        /// </summary>
        public class CollectionChangedEventArgs : EventArgs, IVectorChangedEventArgs
        {
            public CollectionChange CollectionChange
            {
                get;
            }

            public uint Index
            {
                get;
            }

            private CollectionChangedEventArgs(CollectionChange collectionChange, uint index)
            {
                CollectionChange = collectionChange;
                Index = index;
            }

            public static CollectionChangedEventArgs ItemAdded(int index)
            {
                return new CollectionChangedEventArgs(CollectionChange.ItemInserted, (uint) index);
            }

            public static CollectionChangedEventArgs ItemRemoved(int index)
            {
                return new CollectionChangedEventArgs(CollectionChange.ItemRemoved, (uint) index);
            }

            public static CollectionChangedEventArgs ItemInserted(int index)
            {
                return new CollectionChangedEventArgs(CollectionChange.ItemInserted, (uint) index);
            }

            public static CollectionChangedEventArgs ItemChanged(int index)
            {
                return new CollectionChangedEventArgs(CollectionChange.ItemChanged, (uint) index);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private class Enumerator : IEnumerator<TObject>
        {
            private ObservableVector<TObject> owner;
            private readonly long revision;
            private bool disposed;
            private int position;
            private TObject current;

            public TObject Current
            {
                get
                {
                    Contract.Ensures(false == disposed);
                    Contract.Ensures(owner.revision == revision);
                    return current;
                }
            }

            object IEnumerator.Current => Current;

            public Enumerator(ObservableVector<TObject> owner, long revision)
            {
                Contract.Requires(null != owner);

                this.owner = owner;
                this.revision = revision;
            }

            public bool MoveNext()
            {
                Contract.Ensures(false == disposed);
                Contract.Ensures(owner.revision == revision);

                if (position >= owner.Count)
                {
                    return false;
                }

                current = owner[position++];

                return true;
            }

            public void Reset()
            {
                Contract.Ensures(false == disposed);
                Contract.Ensures(owner.revision == revision);
                position = 0;
            }

            public void Dispose()
            {
                Dispose(true);
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
                        owner = null;
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