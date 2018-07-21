using Xarivu.Coder.Model.Tracked.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Xarivu.Coder.Model.Tracked
{
    public abstract class TrackedListBase<TId, TTrackedModel> : ITrackedListBase<TId, TTrackedModel>
        where TId : struct
        where TTrackedModel : class, ITrackedModelBase<TId>, new()
    {
        ObservableCollection<TTrackedModel> collection;

        public TrackedListBase(IEnumerable<TTrackedModel> initialList = null)
        {
            if (initialList == null)
            {
                this.collection = new ObservableCollection<TTrackedModel>();
            }
            else
            {
                this.collection = new ObservableCollection<TTrackedModel>(initialList);
                StartTrackingItems(this.collection);
            }

            this.collection.CollectionChanged += Collection_CollectionChanged;
        }

        public TTrackedModel SelectedItem { get; private set; }

        public bool Remove(TId removeId)
        {
            var index = IndexOf(removeId);
            if (index != -1)
            {
                this.collection.RemoveAt(index);
                return true;
            }

            return false;
        }

        public void Move(int oldIndex, int newIndex)
        {
            this.collection.Move(oldIndex, newIndex);
        }

        public int IndexOf(TId findId)
        {
            for (int i = 0; i < this.collection.Count; ++i)
            {
                var id = this.collection[i].Id;
                if (id.Equals(findId))
                {
                    return i;
                }
            }

            return -1;
        }

        public bool Replace(TId replaceId, TTrackedModel item)
        {
            var index = IndexOf(replaceId);
            if (index != -1)
            {
                this.collection[index] = item;
                return true;
            }

            return false;
        }

        public void SetSelection(TTrackedModel item)
        {
            if ((item == null && this.SelectedItem != null) || (item != null && this.SelectedItem == null) ||
                (item != null && this.SelectedItem != null && !item.Id.Equals(this.SelectedItem.Id)))
            {
                var newStartingIndex = -1;
                if (item != null)
                {
                    newStartingIndex = IndexOf(item.Id);
                    if (newStartingIndex == -1)
                    {
                        // Cannot find selected item in tracked list.
                        // Possible error condition.
                        return;
                    }
                }

                var oldStartingIndex = -1;
                if (this.SelectedItem != null)
                {
                    oldStartingIndex = IndexOf(this.SelectedItem.Id);
                }

                var oldSelectedItem = this.SelectedItem;
                this.SelectedItem = item;
                NotifyTrackedListChanged(ChangedEventArgs.CreateSelectionChangedEventArgs(item, oldSelectedItem, newStartingIndex, oldStartingIndex));
            }
        }

        #region IList<TAppModel> implementation

        public bool IsReadOnly { get => false; }

        public int Count { get => this.collection.Count; }

        public TTrackedModel this[int index]
        {
            get => 0 <= index && index < this.collection.Count ? this.collection[index] : null;
            set => this.collection[index] = value;
        }

        public void Add(TTrackedModel item)
        {
            collection.Add(item);
        }

        public bool Remove(TTrackedModel item)
        {
            return this.Remove(item.Id);
        }

        public void RemoveAt(int index)
        {
            if (index >= 0 && index < this.collection.Count)
            {
                this.collection.RemoveAt(index);
            }
        }

        public void Insert(int index, TTrackedModel item)
        {
            collection.Insert(index, item);
        }

        public void Clear()
        {
            this.collection.Clear();
        }

        public int IndexOf(TTrackedModel item)
        {
            return IndexOf(item.Id);
        }

        public bool Contains(TTrackedModel item)
        {
            var index = IndexOf(item);
            return index != -1;
        }

        public void CopyTo(TTrackedModel[] array, int index)
        {
            this.collection.CopyTo(array, index);
        }

        #endregion

        #region IEnumerable<TTrackedModel> implementation

        IEnumerator<TTrackedModel> IEnumerable<TTrackedModel>.GetEnumerator()
        {
            return this.collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.collection.GetEnumerator();
        }

        #endregion

        public List<TTrackedModel> ToTrackedModelList()
        {
            return this.collection.ToList();
        }

        void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs collectionChangedEventArgs)
        {
            var newItems = collectionChangedEventArgs.NewItems?.Cast<TTrackedModel>().ToList();
            var oldItems = collectionChangedEventArgs.OldItems?.Cast<TTrackedModel>().ToList();

            switch (collectionChangedEventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    // Track new items.
                    StartTrackingItems(newItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    // Remove tracking from old items.
                    StopTrackingItems(oldItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    // Track new items and remove tracking from old items.
                    StopTrackingItems(oldItems);
                    StartTrackingItems(newItems);
                    break;
                case NotifyCollectionChangedAction.Move:
                    // No change needed for item tracking.
                    break;
                case NotifyCollectionChangedAction.Reset:
                    // Remove tracking from all items.
                    StopTrackingItems(this.collection);
                    break;
            }

            NotifyTrackedListChanged(ChangedEventArgs.CreateCollectionChangedEventArgs(collectionChangedEventArgs));
        }

        void StartTrackingItems(IList<TTrackedModel> items)
        {
            if (items == null) return;

            foreach (var item in items)
            {
                item.AddWeakEventHandler(Item_PropertyChanged);
            }
        }

        void StopTrackingItems(IList<TTrackedModel> items)
        {
            if (items == null) return;

            foreach (var item in items)
            {
                item.RemoveWeakEventHandler(Item_PropertyChanged);
            }
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var appModel = sender as TTrackedModel;
            var extendedChangedEventArgs = e as ExtendedPropertyChangedEventArgs;
            if (appModel != null && extendedChangedEventArgs != null)
            {
                NotifyTrackedListChanged(ChangedEventArgs.CreateItemChangedEventArgs(appModel, extendedChangedEventArgs));
            }
        }

        #region List change event handler logic

        public event EventHandler<ChangedEventArgs> TrackedListChanged;

        void NotifyTrackedListChanged(ChangedEventArgs eventArgs)
        {
            if (this.TrackedListChanged != null)
            {
                this.TrackedListChanged(this, eventArgs);
            }
        }

        public void AddWeakEventHandler(EventHandler<ChangedEventArgs> eventHandler)
        {
            WeakEventManager<TrackedListBase<TId, TTrackedModel>, ChangedEventArgs>.AddHandler(
                this,
                nameof(TrackedListChanged),
                eventHandler);
        }

        public void RemoveWeakEventHandler(EventHandler<ChangedEventArgs> eventHandler)
        {
            WeakEventManager<TrackedListBase<TId, TTrackedModel>, ChangedEventArgs>.RemoveHandler(
                this,
                nameof(TrackedListChanged),
                eventHandler);
        }

        #endregion List change event handler logic

        #region Child classes

        public class ChangedEventArgs : TrackedListChangedEventArgs<TId, TTrackedModel>
        {
            public static ChangedEventArgs CreateSelectionChangedEventArgs(
                TTrackedModel newSelectedItem,
                TTrackedModel oldSelectedItem,
                int newStartingIndex,
                int oldStartingIndex)
            {
                var eventArgs = new ChangedEventArgs()
                {
                    ChangeEvent = TrackedListChangeEvent.Select,
                    NewItems = newSelectedItem == null ? null : new List<TTrackedModel>() { newSelectedItem },
                    OldItems = oldSelectedItem == null ? null : new List<TTrackedModel>() { oldSelectedItem },
                    NewStartingIndex = newStartingIndex,
                    OldStartingIndex = oldStartingIndex
                };

                return eventArgs;
            }

            public static ChangedEventArgs CreateCollectionChangedEventArgs(
                NotifyCollectionChangedEventArgs collectionChangedEventArgs)
            {
                var newItems = collectionChangedEventArgs.NewItems?.Cast<TTrackedModel>().ToList();
                var oldItems = collectionChangedEventArgs.OldItems?.Cast<TTrackedModel>().ToList();

                var eventArgs = new ChangedEventArgs();

                switch (collectionChangedEventArgs.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        eventArgs.ChangeEvent = TrackedListChangeEvent.Add;
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        eventArgs.ChangeEvent = TrackedListChangeEvent.Remove;
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        eventArgs.ChangeEvent = TrackedListChangeEvent.Replace;
                        break;
                    case NotifyCollectionChangedAction.Move:
                        eventArgs.ChangeEvent = TrackedListChangeEvent.Move;
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        eventArgs.ChangeEvent = TrackedListChangeEvent.Reset;
                        break;
                }

                eventArgs.NewItems = newItems;
                eventArgs.OldItems = oldItems;
                eventArgs.NewStartingIndex = collectionChangedEventArgs.NewStartingIndex;
                eventArgs.OldStartingIndex = collectionChangedEventArgs.OldStartingIndex;

                return eventArgs;
            }

            /// <summary>
            /// Creates an item changed event args that contains change information for an item in the list.
            /// The item that was changed is added to the NewItems list.
            /// </summary>
            /// <param name="changedEventArgs"></param>
            /// <returns></returns>
            public static ChangedEventArgs CreateItemChangedEventArgs(
                TTrackedModel item,
                ExtendedPropertyChangedEventArgs changedEventArgs)
            {
                var eventArgs = new ChangedEventArgs()
                {
                    NewItems = new List<TTrackedModel>() { item },
                    ChangeEvent = TrackedListChangeEvent.ItemChange,
                    ItemChangedEventArgs = changedEventArgs
                };

                return eventArgs;
            }
        }

        #endregion
    }
}
