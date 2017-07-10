using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace LibraProgramming.Windows.Async
{
    /// <summary>
    /// A double-ended queue (deque), which provides O(1) indexed access, O(1) removals from the front and back,
    /// amortized O(1) insertions to the front and back, and O(N) insertions and removals anywhere else
    /// (with the operations getting slower as the index approaches the middle).
    /// </summary>
    /// <typeparam name="T">The type of elements contained in the deque.</typeparam>
    [DebuggerDisplay("Count = {Count}, Capacity = {Capacity}")]
    [DebuggerTypeProxy(typeof(Deque<>.DebugView))]
    internal sealed class Deque<T> : IList<T>, IList
    {
        private const int DefaultCapacity = 8;

        private int revision;
        private T[] items;
        private int offset;


        public bool IsFixedSize { get; } = false;

        bool IList.IsReadOnly { get; } = false;

        bool ICollection<T>.IsReadOnly { get; } = false;

        /// <summary>
        /// Gets a value indicating whether this instance is at full capacity.
        /// </summary>
        public bool IsFull => Capacity == Count;

        /// <summary>
        /// Gets a value indicating whether the buffer is "split" (meaning the beginning of the view is at a later index in <see cref="items" /> than the end).
        /// </summary>
        public bool IsSplit => offset > (Capacity - Count);

        int ICollection<T>.Count => Count;

        public int Count
        {
            get;
            private set;
        }

        int ICollection.Count => Count;

        public bool IsEmpty => 0 == Count;

        /// <summary>
        /// Gets or sets the capacity for this deque. This value must always be greater than zero, and this property cannot be set to a value less than <see cref="Count"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// <c>Capacity</c> cannot be set to a value less than <see cref="Count" />.
        /// </exception>
        public int Capacity
        {
            get
            {
                return items.Length;
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException();
                }

                if (value < Count)
                {
                    throw new ArgumentException();
                }

                if (value == items.Length)
                {
                    return;
                }

                Interlocked.Increment(ref revision);

                var temp = new T[value];

                if (IsSplit)
                {
                    var length = Capacity - offset;
                    Array.Copy(items, offset, temp, 0, length);
                    Array.Copy(items, 0, temp, length, Count - length);
                }
                else
                {
                    Array.Copy(items, offset, temp, 0, Count);
                }

                items = temp;
                offset = 0;
            }
        }

        public bool IsSynchronized { get; } = false;

        public object SyncRoot => this;
        
        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = (T) value;
            }
        }

        public T this[int index]
        {
            get
            {
                EnsureExistingIndexArgument(Count, index);
                return DoGetItem(index);
            }
            set
            {
                EnsureExistingIndexArgument(Count, index);
                DoSetItem(index, value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Deque{T}" /> class.
        /// </summary>
        public Deque()
            : this(DefaultCapacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Deque{T}"/> class with the specified capacity.
        /// </summary>
        /// <param name="capacity">The initial capacity. Must be greater than <c>0</c>.</param>
        public Deque(int capacity)
        {
            if (capacity < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "Capacity must be greater than 0");
            }

            items = new T[capacity];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Deque{T}"/> class with the elements from the specified collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public Deque(IEnumerable<T> collection)
        {
            if (null == collection)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            var count = collection.Count();

            if (count > 0)
            {
                items = new T[count];
                DoInsertRange(0, collection, count);
            }
            else
            {
                items = new T[DefaultCapacity];
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            var age = revision;

            for (var index = 0; index < Count; index++)
            {
                EnsureRevision(age);
                yield return DoGetItem(index);
            }
        }

        public void Append(T item)
        {
            if (null == item)
            {
                throw new ArgumentNullException(nameof(item));
            }

            EnsureCapacity();

            DoAppend(item);
        }

        public T Dequeue()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException();
            }

            return DoRemoveFromTail();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        void ICollection<T>.Add(T item)
        {
            DoInsert(Count, item);
        }
        
        int IList.Add(object value)
        {
            Append((T) value);
            return Count - 1;
        }

        void IList.Clear()
        {
            DoClear();
        }

        bool IList.Contains(object value)
        {
            return Contains((T) value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((T) value);
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (T) value);
        }

        void IList.Remove(object value)
        {
            Remove((T) value);
        }

        void IList.RemoveAt(int index)
        {
            EnsureExistingIndexArgument(Count, index);
            DoRemoveAt(index);
        }

        public void Clear()
        {
            DoClear();
        }

        public bool Contains(T item)
        {
            return 0 <= IndexOf(item);
        }

        public void CopyTo(T[] array, int index)
        {
            if (null == array)
            {
                throw new ArgumentNullException(nameof(array));
            }

            var count = Count;

            EnsureRangeArgument(array.Length, index, count);

            for (var i = 0; i < count; i++)
            {
                array[index + i] = this[i];
            }
        }

        public bool Remove(T item)
        {
            var index = IndexOf(item);

            if (-1 == index)
            {
                return false;
            }

            DoRemoveAt(index);

            return true;
        }

        public void CopyTo(Array array, int index)
        {
            if (null == array)
            {
                throw new ArgumentNullException(nameof(array));
            }

            var count = Count;

            EnsureRangeArgument(array.Length, index, count);

            for (var i = 0; i < count; i++)
            {
                try
                {
                    array.SetValue(this[i], index + i);
                }
                catch (InvalidCastException exception)
                {
                    throw new ArgumentException("", exception);
                }
            }
        }

        public int IndexOf(T item)
        {
            var comparer = EqualityComparer<T>.Default;
            var index = 0;

            foreach (var entry in this)
            {
                if (comparer.Equals(entry, item))
                {
                    return index;
                }

                index++;
            }

            return -1;
        }

        public void Insert(int index, T item)
        {
            EnsureNewIndexArgument(Count, index);
            DoInsert(index, item);
        }

        public void RemoveAt(int index)
        {
            EnsureExistingIndexArgument(Count, index);
            DoRemoveAt(index);
        }

        private int DequeIndexToItemsIndex(int index)
        {
            return (index + offset) % Capacity;
        }

        private int PreDecrement(int value)
        {
            offset -= value;

            if (offset < 0)
            {
                offset += Capacity;
            }

            return offset;
        }

        private int PostIncrement(int value)
        {
            var result = offset;

            offset += value;
            offset %= Capacity;

            return result;
        }

        private void EnsureExistingIndexArgument(int value, int index)
        {
            if (index < 0 || index >= value)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "");
            }
        }

        private void EnsureNewIndexArgument(int length, int index)
        {
            if (0 > index || index > length)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "");
            }
        }

        private void EnsureRangeArgument(int length, int distance, int count)
        {
            if (distance < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(distance), distance, "");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, "");
            }

            if ((length - distance) < count)
            {
                throw new ArgumentException("");
            }
        }

        private void EnsureCapacity()
        {
            if (IsFull)
            {
                Capacity *= 2;
            }
        }

        private void EnsureRevision(int value)
        {
            if (revision != value)
            {
                throw new InvalidOperationException();
            }
        }

        private void DoClear()
        {
            Interlocked.Increment(ref revision);
            offset = 0;
            Count = 0;
            Array.Clear(items, 0, items.Length);
        }

        private T DoGetItem(int index)
        {
            return items[DequeIndexToItemsIndex(index)];
        }

        private void DoSetItem(int index, T value)
        {
            Interlocked.Increment(ref revision);
            items[DequeIndexToItemsIndex(index)] = value;
        }

        private void DoInsert(int index, T item)
        {
            EnsureCapacity();

            if (0 == index)
            {
                DoPrepend(item);
                return;
            }

            if (Count == index)
            {
                DoAppend(item);
                return;
            }

            DoInsertRange(index, new[] { item }, 1);
        }

        private void DoRemoveAt(int index)
        {
            if (0 == index)
            {
                DoRemoveFromHead();
                return;
            }

            if ((Count - 1) == index)
            {
                DoRemoveFromTail();
                return;
            }

            DoRemoveRange(index, 1);
        }

        private void DoRemoveRange(int index, int count)
        {
            if (0 == index)
            {
                PostIncrement(count);
                Count -= count;

                return;
            }

            if ((Count - count) == index)
            {
                Count -= count;
                return;
            }

            if ((Count / 2) > ((count / 2) + index))
            {
                var cc = index;
                var wi = count;

                for (var position = cc-1; position != -1; position--)
                {
                    items[DequeIndexToItemsIndex(wi + position)] = items[DequeIndexToItemsIndex(position)];
                }

                PostIncrement(count);
            }
            else
            {
                var cc = Count - count - index;
                var ri = index + count;

                for (var position = 0; position != cc; position++)
                {
                    items[DequeIndexToItemsIndex(index + position)] = items[DequeIndexToItemsIndex(ri + position)];
                }
            }

            Count -= count;
        }

        private void DoPrepend(T item)
        {
            items[PreDecrement(1)] = item;
            Count++;
        }

        private void DoAppend(T item)
        {
            items[DequeIndexToItemsIndex(Count)] = item;
            Count++;
        }

        private void DoInsertRange(int index, IEnumerable<T> collection, int count)
        {
            Interlocked.Increment(ref revision);

            // Make room in the existing list
            if (index < Count / 2)
            {
                // Inserting into the first half of the list

                // Move lower items down: [0, index) -> [Capacity - collectionCount, Capacity - collectionCount + index)
                // This clears out the low "index" number of items, moving them "collectionCount" places down;
                //   after rotation, there will be a "collectionCount"-sized hole at "index".
                var copyCount = index;
                var writeIndex = Capacity - count;

                for (var j = 0; j != copyCount; ++j)
                {
                    items[DequeIndexToItemsIndex(writeIndex + j)] = items[DequeIndexToItemsIndex(j)];
                }

                // Rotate to the new view
                PreDecrement(count);
            }
            else
            {
                // Inserting into the second half of the list

                // Move higher items up: [index, count) -> [index + collectionCount, collectionCount + count)
                var copyCount = Count - index;
                var writeIndex = index + count;

                for (var j = copyCount - 1; j != -1; --j)
                {
                    items[DequeIndexToItemsIndex(writeIndex + j)] = items[DequeIndexToItemsIndex(index + j)];
                }
            }

            // Copy new items into place
            var i = index;

            foreach (var item in collection)
            {
                items[DequeIndexToItemsIndex(i)] = item;
                ++i;
            }

            // Adjust valid count
            Count += count;
        }

        private T DoRemoveFromTail()
        {
            var item = items[DequeIndexToItemsIndex(Count - 1)];

            Count--;

            return item;
        }

        private T DoRemoveFromHead()
        {
            Count--;
            return items[PostIncrement(1)];
        }

        [DebuggerNonUserCode]
        private sealed class DebugView
        {
            private readonly Deque<T> deque;

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public T[] Items
            {
                get
                {
                    var array = new T[deque.Count];

                    deque.CopyTo(array, 0);

                    return array;
                }
            }

            public DebugView(Deque<T> deque)
            {
                this.deque = deque;
            }
        }
    }
}