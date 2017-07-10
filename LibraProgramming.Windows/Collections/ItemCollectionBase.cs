using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using LibraProgramming.Windows.Infrastructure;

namespace LibraProgramming.Windows.Collections
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    public class ItemCollectionBase<TObject> : CollectionBase, IList<TObject>, INotifyCollectionChanged
        where TObject : class
    {
        private readonly WeakEvent<NotifyCollectionChangedEventHandler> collectionChanged; 
        private int revision;

        /// <summary>
        /// Получает значение, указывающее, является ли объект <see cref="T:System.Collections.Generic.ICollection`1"/> доступным только для чтения.
        /// </summary>
        /// <returns>
        /// Значение true, если <see cref="T:System.Collections.Generic.ICollection`1"/> доступна только для чтения; в противном случае — значение false.
        /// </returns>
        public bool IsReadOnly => InnerList.IsReadOnly;

        /// <summary>
        /// Получает или задает элемент с указанным индексом.
        /// </summary>
        /// <returns>
        /// Элемент с заданным индексом.
        /// </returns>
        /// <param name="index">
        /// Отсчитываемый с нуля индекс получаемого или задаваемого элемента.
        /// </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// Параметр <paramref name="index"/> не является допустимым индексом в списке <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// Свойство задано, и объект <see cref="T:System.Collections.Generic.IList`1"/> доступен только для чтения.
        /// </exception>
        public TObject this[int index]
        {
            get
            {
                return (TObject) InnerList[index];
            }
            set
            {
                if (InnerList[index] == value)
                {
                    return;
                }

                var old = InnerList[index];

                InnerList[index] = value;

                DoCollectionChanged(EventArgs.ItemReplaced(index, old, value));
            }
        }

        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add
            {
                collectionChanged.AddHandler(value);
            }
            remove
            {
                collectionChanged.RemoveHandler(value);
            }
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:System.Collections.CollectionBase"/> с начальной емкостью по умолчанию.
        /// </summary>
        public ItemCollectionBase()
        {
            collectionChanged = new WeakEvent<NotifyCollectionChangedEventHandler>();
        }

        /// <summary>
        /// Возвращает перечислитель, выполняющий перебор элементов в коллекции.
        /// </summary>
        /// <returns>
        /// Перечислитель, который можно использовать для итерации по коллекции.
        /// </returns>
        IEnumerator<TObject> IEnumerable<TObject>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        /// Добавляет элемент в коллекцию <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">
        /// Объект, добавляемый в коллекцию <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </param>
        /// <exception cref="T:System.NotSupportedException">
        /// Объект <see cref="T:System.Collections.Generic.ICollection`1"/> доступен только для чтения.
        /// </exception>
        public void Add(TObject item)
        {
            if (null == item)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var index = InnerList.Add(item);

            DoCollectionChanged(EventArgs.ItemAdded(index, item));
        }

        /// <summary>
        /// Определяет, содержит ли коллекция <see cref="T:System.Collections.Generic.ICollection`1"/> указанное значение.
        /// </summary>
        /// <returns>
        /// Значение true, если объект <paramref name="item"/> найден в <see cref="T:System.Collections.Generic.ICollection`1"/>; в противном случае — значение false.
        /// </returns>
        /// <param name="item">
        /// Объект, который требуется найти в <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </param>
        public bool Contains(TObject item)
        {
            if (null == item)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return InnerList.Contains(item);
        }

        /// <summary>
        /// Копирует элементы <see cref="T:System.Collections.Generic.ICollection`1"/> в массив <see cref="T:System.Array"/>, начиная с указанного индекса <see cref="T:System.Array"/>.
        /// </summary>
        /// <param name="array">
        /// Одномерный массив <see cref="T:System.Array"/>, в который копируются элементы из интерфейса <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// Индексация в массиве <see cref="T:System.Array"/> должна начинаться с нуля.
        /// </param>
        /// <param name="arrayIndex">
        /// Индекс (с нуля) в массиве <paramref name="array"/>, с которого начинается копирование.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// Параметр <paramref name="array"/> имеет значение null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// Значение параметра <paramref name="arrayIndex"/> меньше 0.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// Количество элементов в исходной коллекции <see cref="T:System.Collections.Generic.ICollection`1"/> превышает
        /// доступное место в целевом массиве <paramref name="array"/>, начиная с индекса <paramref name="arrayIndex"/> до конца массива.
        /// </exception>
        public void CopyTo(TObject[] array, int arrayIndex)
        {
            InnerList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Удаляет первый экземпляр указанного объекта из коллекции <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// Значение true, если объект <paramref name="item"/> успешно удален из <see cref="T:System.Collections.Generic.ICollection`1"/>,
        /// в противном случае — значение false.Этот метод также возвращает значение false, если параметр <paramref name="item"/>
        /// не найден в исходном интерфейсе <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <param name="item">
        /// Объект, который необходимо удалить из коллекции <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </param>
        /// <exception cref="T:System.NotSupportedException">
        /// Объект <see cref="T:System.Collections.Generic.ICollection`1"/> доступен только для чтения.
        /// </exception>
        public bool Remove(TObject item)
        {
            if (null == item)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var index = InnerList.IndexOf(item);

            if (-1 == index)
            {
                return false;
            }

            var count = InnerList.Count;

            InnerList.RemoveAt(index);

            DoCollectionChanged(EventArgs.ItemRemoved(index, item));

            return count > InnerList.Count;
        }

        /// <summary>
        /// Определяет индекс заданного элемента коллекции <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </summary>
        /// <returns>
        /// Индекс <paramref name="item"/> если он найден в списке; в противном случае его значение равно -1.
        /// </returns>
        /// <param name="item">
        /// Объект, который требуется найти в <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </param>
        public int IndexOf(TObject item)
        {
            if (null == item)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return InnerList.IndexOf(item);
        }

        /// <summary>
        /// Вставляет элемент в список <see cref="T:System.Collections.Generic.IList`1"/> по указанному индексу.
        /// </summary>
        /// <param name="index">
        /// Индекс (с нуля), по которому вставляется <paramref name="item"/>.
        /// </param>
        /// <param name="item">
        /// Объект, вставляемый в <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// Параметр <paramref name="index"/> не является допустимым индексом в списке <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// Объект <see cref="T:System.Collections.Generic.IList`1"/> доступен только для чтения.
        /// </exception>
        public void Insert(int index, TObject item)
        {
            if (null == item)
            {
                throw new ArgumentNullException(nameof(item));
            }

            InnerList.Insert(index, item);

            DoCollectionChanged(EventArgs.ItemInserted(index, item));
        }

        private void DoCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            revision++;
            collectionChanged?.Invoke(this, e);
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class Enumerator : IEnumerator<TObject>
        {
            private readonly ItemCollectionBase<TObject> collection;
            private readonly int revision;
            private bool disposed;
            private int position = -1;
            private TObject current;

            /// <summary>
            /// Возвращает элемент коллекции, соответствующий текущей позиции перечислителя.
            /// </summary>
            /// <returns>
            /// Элемент коллекции, соответствующий текущей позиции перечислителя.
            /// </returns>
            public TObject Current
            {
                get
                {
                    EnsureNotDisposed();
                    EnsureNotModified();

                    return current;
                }
                private set
                {
                    current = value;
                }
            }

            /// <summary>
            /// Получает текущий элемент в коллекции.
            /// </summary>
            /// <returns>
            /// Текущий элемент в коллекции.
            /// </returns>
            object IEnumerator.Current => Current;

            /// <summary>
            /// Инициализирует новый экземпляр класса <see cref="T:System.Object"/>.
            /// </summary>
            internal Enumerator(ItemCollectionBase<TObject> collection)
            {
                this.collection = collection;
                revision = collection.revision;
            }

            /// <summary>
            /// Перемещает перечислитель к следующему элементу коллекции.
            /// </summary>
            /// <returns>
            /// Значение true, если перечислитель был успешно перемещен к следующему элементу; значение false, если перечислитель достиг конца коллекции.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public bool MoveNext()
            {
                EnsureNotDisposed();
                EnsureNotModified();

                var count = collection.Count;
                var canMoveNext = 0 < count && position < (count - 1);

                if (canMoveNext)
                {
                    Current = collection[position++];
                }

                return canMoveNext;
            }

            /// <summary>
            /// Устанавливает перечислитель в его начальное положение, т. е. перед первым элементом коллекции.
            /// </summary>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public void Reset()
            {
                EnsureNotDisposed();
                EnsureNotModified();

                position = 0 >= collection.Count ? -1 : 0;
            }

            /// <summary>
            /// Выполняет определяемые приложением задачи, связанные с удалением, высвобождением или сбросом неуправляемых ресурсов.
            /// </summary>
            public void Dispose()
            {
                Dispose(true);
            }

            private void EnsureNotDisposed()
            {
                if (disposed)
                {
                    throw new ObjectDisposedException("");
                }
            }

            private void EnsureNotModified()
            {
                if (revision != collection.revision)
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
                        Current = null;
                        position = -1;
                    }
                }
                finally
                {
                    disposed = true;
                }
            }
        }

        private static class EventArgs
        {
            public static NotifyCollectionChangedEventArgs ItemAdded(int index, object obj)
            {
                return ItemInserted(index, obj);
            }

            public static NotifyCollectionChangedEventArgs ItemInserted(int index, object obj)
            {
                return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, obj, index);
            }

            public static NotifyCollectionChangedEventArgs ItemRemoved(int index, object obj)
            {
                return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, obj, index);
            }

            public static NotifyCollectionChangedEventArgs ItemReplaced(int index, object old, object obj)
            {
                return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, obj, old, index);
            }
        }
    }
}