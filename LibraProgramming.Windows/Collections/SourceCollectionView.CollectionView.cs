using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using LibraProgramming.Windows.Infrastructure;

namespace LibraProgramming.Windows.Collections
{
    public partial class SourceCollectionView
    {
        private interface IRevisionProvider
        {
            /*IEnumerable Source
            {
                get;
            }*/

            /*Predicate<object> Filter
            {
                get;
            }*/

            int Revision
            {
                get;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class CollectionView : ICollectionView, IDeferrableRefresh, IRevisionProvider
        {
            private EnumerableSourceIterator iterator;
            private Predicate<object> filter;
            private bool isCurrentBeforeFirst;
            private bool isCurrentAfterLast;
//            private object currentItem;
//            private int currentPosition;
            private int revision;

            /// <summary>
            /// 
            /// </summary>
            public Predicate<object> Filter
            {
                get
                {
                    return filter;
                }
                set
                {
                    filter = value;
                    Reset();
                }
            }

            /// <summary>
            /// Получает число элементов, содержащихся в интерфейсе <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </summary>
            /// <returns>
            /// Число элементов, содержащихся в интерфейсе <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </returns>
            public int Count
            {
                get;
                private set;
            }

            /// <summary>
            /// Получает значение, указывающее, является ли объект <see cref="T:System.Collections.Generic.ICollection`1"/> доступным только для чтения.
            /// </summary>
            /// <returns>
            /// Значение true, если <see cref="T:System.Collections.Generic.ICollection`1"/> доступна только для чтения; в противном случае — значение false.
            /// </returns>
            public bool IsReadOnly => iterator.IsReadOnly;

            /// <summary>
            /// Возвращает все группы коллекций, связанные с представлением.
            /// </summary>
            /// <returns>
            /// Коллекция векторов возможных представлений.
            /// </returns>
            public IObservableVector<object> CollectionGroups
            {
                get;
            }

            /// <summary>
            /// Получает текущий элемент в представлении.
            /// </summary>
            /// <returns>
            /// Текущий элемент в представлении или значение null, если текущий элемент отсутствует.
            /// </returns>
            public object CurrentItem
            {
                get;
                private set;
            }

            /// <summary>
            /// Получает порядковый номер элемента CurrentItem в представлении.
            /// </summary>
            /// <returns>
            /// Порядковый номер элемента CurrentItem в представлении.
            /// </returns>
            public int CurrentPosition
            {
                get;
                private set;
            }

            /// <summary>
            /// Получает значение-метку, которое поддерживает реализации инкрементной загрузки. См. также раздел LoadMoreItemsAsync.
            /// </summary>
            /// <returns>
            /// Значение true, если дополнительные выгруженные элементы остаются в представлении; в противном случае - значение false.
            /// </returns>
            public bool HasMoreItems => false;

            /// <summary>
            /// Получает значение, показывающее, находится ли элемент CurrentItem представления за концом коллекции.
            /// </summary>
            /// <returns>
            /// Значение true, если свойство CurrentItem представления находится за пределами конца коллекции; в противном случае - false.
            /// </returns>
            public bool IsCurrentAfterLast
            {
                get;
                private set;
            }

            /// <summary>
            /// Получает значение, указывающее, находится ли элемент CurrentItem представления за началом коллекции.
            /// </summary>
            /// <returns>
            /// Значение true, если свойство CurrentItem представления находится за пределами начала коллекции; в противном случае - значение false.
            /// </returns>
            public bool IsCurrentBeforeFirst
            {
                get;
                private set;
            }

            public IEnumerable Source
            {
                get
                {
                    return iterator?.Source;
                }
                set
                {
                    if (null != iterator && Object.Equals(value, iterator.Source))
                    {
                        return;
                    }

                    DiscardSourceSubscription();

                    iterator = CreateIterator(value);

                    SubscribeToSourceChanges();
                    Reset();
                }
            }

            /// <summary>
            /// Получает или задает элемент с указанным индексом.
            /// </summary>
            /// <returns>
            /// Элемент с заданным индексом.
            /// </returns>
            /// <param name="index">Отсчитываемый с нуля индекс получаемого или задаваемого элемента.</param><exception cref="T:System.ArgumentOutOfRangeException">Параметр <paramref name="index"/> не является допустимым индексом в списке <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">Свойство задано, и объект <see cref="T:System.Collections.Generic.IList`1"/> доступен только для чтения.</exception>
            public object this[int index]
            {
                get
                {
                    return iterator[index];
                }
                set
                {
//                    EnsureNotReadOnly();
                    iterator[index] = value;
                }
            }

            int IDeferrableRefresh.DeferLevel
            {
                get;
                set;
            }

            int IRevisionProvider.Revision => revision;

            public event CurrentChangingEventHandler CurrentChanging;

            public event EventHandler<object> CurrentChanged;

            public event VectorChangedEventHandler<object> VectorChanged;

            public CollectionView()
            {
                CollectionGroups = new ObservableVector<object>();
            }

            /// <summary>
            /// Возвращает перечислитель, выполняющий перебор элементов коллекции.
            /// </summary>
            /// <returns>
            /// Интерфейс <see cref="T:System.Collections.Generic.IEnumerator`1"/>, который может использоваться для перебора элементов коллекции.
            /// </returns>
            public IEnumerator<object> GetEnumerator()
            {
                return iterator.GetEnumerator();
            }

            /// <summary>
            /// Возвращает перечислитель, который осуществляет итерацию по коллекции.
            /// </summary>
            /// <returns>
            /// Объект <see cref="T:System.Collections.IEnumerator"/>, который может использоваться для перебора коллекции.
            /// </returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            /// <summary>
            /// Добавляет элемент в коллекцию <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </summary>
            /// <param name="item">Объект, добавляемый в коллекцию <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">Объект <see cref="T:System.Collections.Generic.ICollection`1"/> доступен только для чтения.</exception>
            public void Add(object item)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Удаляет все элементы из интерфейса <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </summary>
            /// <exception cref="T:System.NotSupportedException">Объект <see cref="T:System.Collections.Generic.ICollection`1"/> доступен только для чтения.</exception>
            public void Clear()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Определяет, содержит ли коллекция <see cref="T:System.Collections.Generic.ICollection`1"/> указанное значение.
            /// </summary>
            /// <returns>
            /// Значение true, если объект <paramref name="item"/> найден в <see cref="T:System.Collections.Generic.ICollection`1"/>; в противном случае — значение false.
            /// </returns>
            /// <param name="item">Объект, который требуется найти в <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
            public bool Contains(object item)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Копирует элементы <see cref="T:System.Collections.Generic.ICollection`1"/> в массив <see cref="T:System.Array"/>, начиная с указанного индекса <see cref="T:System.Array"/>.
            /// </summary>
            /// <param name="array">Одномерный массив <see cref="T:System.Array"/>, в который копируются элементы из интерфейса <see cref="T:System.Collections.Generic.ICollection`1"/>. Индексация в массиве <see cref="T:System.Array"/> должна начинаться с нуля.</param><param name="arrayIndex">Индекс (с нуля) в массиве <paramref name="array"/>, с которого начинается копирование.</param><exception cref="T:System.ArgumentNullException">Параметр <paramref name="array"/> имеет значение null.</exception><exception cref="T:System.ArgumentOutOfRangeException">Значение параметра <paramref name="arrayIndex"/> меньше 0.</exception><exception cref="T:System.ArgumentException">Количество элементов в исходной коллекции <see cref="T:System.Collections.Generic.ICollection`1"/> превышает доступное место в целевом массиве <paramref name="array"/>, начиная с индекса <paramref name="arrayIndex"/> до конца массива.</exception>
            public void CopyTo(object[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Удаляет первый экземпляр указанного объекта из коллекции <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </summary>
            /// <returns>
            /// Значение true, если объект <paramref name="item"/> успешно удален из <see cref="T:System.Collections.Generic.ICollection`1"/>, в противном случае — значение false. Этот метод также возвращает значение false, если параметр <paramref name="item"/> не найден в исходном интерфейсе <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </returns>
            /// <param name="item">Объект, который необходимо удалить из коллекции <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">Объект <see cref="T:System.Collections.Generic.ICollection`1"/> доступен только для чтения.</exception>
            public bool Remove(object item)
            {
                EnsureNotReadOnly();
                throw new NotImplementedException();
            }

            /// <summary>
            /// Определяет индекс заданного элемента коллекции <see cref="T:System.Collections.Generic.IList`1"/>.
            /// </summary>
            /// <returns>
            /// Индекс <paramref name="item"/> если он найден в списке; в противном случае его значение равно -1.
            /// </returns>
            /// <param name="item">Объект, который требуется найти в <see cref="T:System.Collections.Generic.IList`1"/>.</param>
            public int IndexOf(object item)
            {
                return iterator.IndexOf(item);
            }

            /// <summary>
            /// Вставляет элемент в список <see cref="T:System.Collections.Generic.IList`1"/> по указанному индексу.
            /// </summary>
            /// <param name="index">Индекс (с нуля), по которому вставляется <paramref name="item"/>.</param><param name="item">Объект, вставляемый в <see cref="T:System.Collections.Generic.IList`1"/>.</param><exception cref="T:System.ArgumentOutOfRangeException">Параметр <paramref name="index"/> не является допустимым индексом в списке <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">Объект <see cref="T:System.Collections.Generic.IList`1"/> доступен только для чтения.</exception>
            public void Insert(int index, object item)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Удаляет элемент <see cref="T:System.Collections.Generic.IList`1"/> по указанному индексу.
            /// </summary>
            /// <param name="index">Отсчитываемый от нуля индекс удаляемого элемента.</param><exception cref="T:System.ArgumentOutOfRangeException">Параметр <paramref name="index"/> не является допустимым индексом в списке <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">Объект <see cref="T:System.Collections.Generic.IList`1"/> доступен только для чтения.</exception>
            public void RemoveAt(int index)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Задает указанный элемент в качестве CurrentItem в представлении.
            /// </summary>
            /// <returns>
            /// Значение true, если полученный элемент CurrentItem принадлежит представлению; в противном случае - значение false.
            /// </returns>
            /// <param name="item">Элемент, устанавливаемый в качестве CurrentItem.</param>
            public bool MoveCurrentTo(object item)
            {
                var index = iterator.IndexOf(item);

                if (0 > index)
                {
                    return false;
                }

                return MoveCurrentToPosition(index);
            }

            /// <summary>
            /// Задает элемент по заданному индексу в качестве CurrentItem в представлении.
            /// </summary>
            /// <returns>
            /// Значение true, если полученный элемент CurrentItem принадлежит представлению; в противном случае - значение false.
            /// </returns>
            /// <param name="index">Индекс перемещаемого элемента.</param>
            public bool MoveCurrentToPosition(int index)
            {
                if (false == CanChangeCurrent())
                {
                    return false;
                }
                
                if (-1 > index || index > Count)
                {
                    return false;
                }

                UpdateCurrencyAndPosition(index);

                return true;
            }

            /// <summary>
            /// Задает первый элемент в представлении в качестве CurrentItem.
            /// </summary>
            /// <returns>
            /// Значение true, если полученный элемент CurrentItem принадлежит представлению; в противном случае - значение false.
            /// </returns>
            public bool MoveCurrentToFirst()
            {
                return MoveCurrentToPosition(0);
            }

            /// <summary>
            /// Задает последний элемент в представлении в качестве CurrentItem.
            /// </summary>
            /// <returns>
            /// Значение true, если полученный элемент CurrentItem принадлежит представлению; в противном случае - значение false.
            /// </returns>
            public bool MoveCurrentToLast()
            {
                return MoveCurrentToPosition(Count - 1);
            }

            /// <summary>
            /// Задает элемент после CurrentItem в представлении в качестве элемента CurrentItem.
            /// </summary>
            /// <returns>
            /// Значение true, если полученный элемент CurrentItem принадлежит представлению; в противном случае - значение false.
            /// </returns>
            public bool MoveCurrentToNext()
            {
                return MoveCurrentToPosition(CurrentPosition + 1);
            }

            /// <summary>
            /// Устанавливает элемент перед элементом CurrentItem в качестве CurrentItem.
            /// </summary>
            /// <returns>
            /// Значение true, если полученный элемент CurrentItem принадлежит представлению; в противном случае - значение false.
            /// </returns>
            public bool MoveCurrentToPrevious()
            {
                return MoveCurrentToPosition(CurrentPosition - 1);
            }

            /// <summary>
            /// Инициализирует инкрементную загрузку из представления.
            /// </summary>
            /// <returns>
            /// Свернутые результаты операции загрузки.
            /// </returns>
            /// <param name="count">Число загружаемых элементов.</param>
            public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
            {
                throw new NotImplementedException();
            }

            public IDisposable DeferRefresh()
            {
                return new RefreshDeferToken(this);
            }

            void IDeferrableRefresh.Refresh()
            {
                if (0 < ((IDeferrableRefresh) this).DeferLevel)
                {
                    return;
                }

                using (var context = LockUpdates())
                {
                    Count = iterator.GetCount();
                    MoveCurrentToPosition(-1);

                    /*if (IsCurrentBeforeFirst)
                    {
                    }
                    else if (IsCurrentAfterLast)
                    {
                        MoveCurrentToPosition(Count);
                    }
                    else if (null != CurrentItem)
                    {
                        MoveCurrentTo(CurrentItem);
                    }*/

//                    context.Complete();

                }

                RaiseCollectionChanged(NotifyVectorChangedEventArgs.Reset());

                //                RaiseCollectionChanged(NotifyVectorChangedEventArgs.Reset());
                //                RaisePropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
            }

            private void Reset()
            {
                ((IDeferrableRefresh) this).Refresh();
            }

            private void EnsureNotReadOnly()
            {
                if (IsReadOnly)
                {
                    throw new InvalidOperationException();
                }
            }

            private bool CanChangeCurrent()
            {
                var e = new CurrentChangingEventArgs();

                CurrentChanging?.Invoke(this, e);

                return false == e.Cancel;
            }

            private void RaiseCollectionChanged(IVectorChangedEventArgs e)
            {
                if (((IDeferrableRefresh) this).DeferLevel > 0)
                {
                    return;
                }

                VectorChanged?.Invoke(this, e);
            }

            private void RaiseCurrentChanged()
            {
                if (((IDeferrableRefresh) this).DeferLevel > 0)
                {
                    return;
                }

                CurrentChanged?.Invoke(this, CurrentItem);
            }

            private IUnitOfWork LockUpdates()
            {
                return new UpdateContext(this);
            }

            private void DiscardSourceSubscription()
            {
                var collection = Source as INotifyCollectionChanged;

                if (null == collection)
                {
                    return;
                }

                collection.CollectionChanged -= OnSourceCollectionChanged;
            }

            private void SubscribeToSourceChanges()
            {
                var collection = Source as INotifyCollectionChanged;

                if (null == collection)
                {
                    return;
                }

                collection.CollectionChanged += OnSourceCollectionChanged;
            }

            private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        {
                            foreach (var item in e.NewItems)
                            {
                                using (var context = LockUpdates())
                                {
                                    var index = iterator.IndexOf(item);

                                    if (0 > index)
                                    {
                                        return;
                                    }

                                    Count++;

                                    UpdateCurrencyAndPosition(index);

                                    context.Complete();

                                    RaiseCollectionChanged(NotifyVectorChangedEventArgs.Inserted(index));
                                }
                            }

                            break;
                        }
                }
            }

            private void UpdateCurrencyAndPosition(int index)
            {
                IsCurrentBeforeFirst = 0 > index;
                IsCurrentAfterLast = Count <= index;
                CurrentPosition = index;
                CurrentItem = false == IsCurrentBeforeFirst && false == IsCurrentAfterLast ? iterator[index] : null;
            }

            private EnumerableSourceIterator CreateIterator(IEnumerable source)
            {
                var list = source as IList;

                if (null == Filter && null != list)
                {
                    return new ListSourceIterator(this, list);
                }

                return new EnumerableSourceIterator(this, source, Filter);
            }

            /*
            private void EnsureCurrentPosition()
            {
                if (null == owner.list || owner.list.Count < 1)
                {
                    throw new IndexOutOfRangeException();
                }
            }

            private bool CanChangeCurrent()
            {
                var e = new CurrentChangingEventArgs(true);

                CurrentChanging?.Invoke(this, e);

                return false == e.Cancel;
            }

            private void DoCurrentChanged(object current)
            {
                CurrentChanged?.Invoke(this, current);
            }*/

            /// <summary>
            /// 
            /// </summary>
            private class NotifyVectorChangedEventArgs : EventArgs, IVectorChangedEventArgs
            {
                /// <summary>
                /// 
                /// </summary>
                public CollectionChange CollectionChange
                {
                    get;
                }

                /// <summary>
                /// 
                /// </summary>
                public uint Index
                {
                    get;
                }

                public static IVectorChangedEventArgs Reset()
                {
                    return new NotifyVectorChangedEventArgs(CollectionChange.Reset);
                }

                public static IVectorChangedEventArgs Inserted(int index)
                {
                    return new NotifyVectorChangedEventArgs(CollectionChange.ItemInserted, (uint) index);
                }

                private NotifyVectorChangedEventArgs(CollectionChange change)
                    : this(change, 0)
                {
                }

                private NotifyVectorChangedEventArgs(CollectionChange change, uint index)
                {
                    CollectionChange = change;
                    Index = index;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            private class UpdateContext : IUnitOfWork
            {
                private readonly int currentPosition;
                private readonly object currentItem;
                private readonly bool isCurrentBeforeFirst;
                private readonly bool isCurrentAfterLast;
                private readonly CollectionView view;

                public UpdateContext(CollectionView view)
                {
                    this.view = view;
                    currentPosition = view.CurrentPosition;
                    currentItem = view.CurrentItem;
                    isCurrentBeforeFirst = view.IsCurrentBeforeFirst;
                    isCurrentAfterLast = view.IsCurrentAfterLast;
                }

                public void Complete()
                {
                    if (currentPosition != view.CurrentPosition)
                    {
                        view.RaiseCurrentChanged();
                    }

                    if (isCurrentBeforeFirst != view.IsCurrentBeforeFirst)
                    {
//                        view.RaisePropertyChanged(new PropertyChangedEventArgs(nameof(view.IsCurrentBeforeFirst)));
                    }

                    if (isCurrentAfterLast != view.IsCurrentAfterLast)
                    {
//                        view.RaisePropertyChanged(new PropertyChangedEventArgs(nameof(view.IsCurrentAfterLast)));
                    }
                }

                public void Dispose()
                {
                    ;
                }
            }
        }
    }
}