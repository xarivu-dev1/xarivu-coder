using Xarivu.Coder.Model.Tracked;
using Xarivu.Coder.Model.Tracked.Interface;
using Xarivu.Coder.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace Xarivu.Coder.ViewModel.DataGridBinding
{
    public abstract class DataGridBindingViewModel<TId, TTrackedModel> : NotifyChangeBase, IDataGridBindingViewModel
        where TId : struct
        where TTrackedModel : class, ITrackedModelBase<TId>, new()
    {
       TrackedListBase<TId, TTrackedModel> trackedList;

        public DataGridBindingViewModel(TrackedListBase<TId, TTrackedModel> trackedList)
        {
            if (trackedList == null)
            {
                trackedList = CreateEmptyList();
            }

            this.trackedList = trackedList;

            this.ShowListCommand = new DelegateCommand(p => ShowListView());
            this.ShowGroupCommand = new DelegateCommand(p => ShowGroupView());
            this.ToggleGroupExpansionCommand = new DelegateCommand(p => ToggleGroupExpansion());

            this.RemovedItems = new List<TTrackedModel>();

            this.Items = new ObservableCollection<TTrackedModel>(trackedList);

            // Register property changed event handlers on items.
            MultipleItemsAddedToCollection(trackedList, false);

            UpdateCommandCanExecute();

            this.PropertyChanged += DataGridBindingViewModel_PropertyChanged;
            this.Items.CollectionChanged += ObservableCollection_CollectionChanged;

            this.trackedList.AddWeakEventHandler(ITrackedList_CollectionChanged);
        }

        public event Action CollectionChanged;

        public List<TTrackedModel> RemovedItems { get; private set; }

        public DelegateCommand ShowListCommand { get; private set; }
        public DelegateCommand ShowGroupCommand { get; private set; }
        public DelegateCommand ToggleGroupExpansionCommand { get; private set; }

        #region IsReadOnly
        bool __IsReadOnly;
        public bool IsReadOnly
        {
            get
            {
                return this.__IsReadOnly;
            }

            set
            {
                if (this.__IsReadOnly != value)
                {
                    this.__IsReadOnly = value;
                    NotifyPropertyChanged(nameof(IsReadOnly));
                }
            }
        }
        #endregion

        #region CanUserAddRows
        bool __CanUserAddRows;
        public bool CanUserAddRows
        {
            get
            {
                return this.__CanUserAddRows;
            }

            set
            {
                if (this.__CanUserAddRows != value)
                {
                    this.__CanUserAddRows = value;
                    NotifyPropertyChanged(nameof(CanUserAddRows));
                }
            }
        }
        #endregion

        #region CanUserDeleteRows
        bool __CanUserDeleteRows;
        public bool CanUserDeleteRows
        {
            get
            {
                return this.__CanUserDeleteRows;
            }

            set
            {
                if (this.__CanUserDeleteRows != value)
                {
                    this.__CanUserDeleteRows = value;
                    NotifyPropertyChanged(nameof(CanUserDeleteRows));
                }
            }
        }
        #endregion

        #region Items
        ObservableCollection<TTrackedModel> __Items;
        public ObservableCollection<TTrackedModel> Items
        {
            get
            {
                return this.__Items;
            }

            set
            {
                if (this.__Items != value)
                {
                    this.__Items = value;
                    this.ItemCollectionView = CollectionViewSource.GetDefaultView(value);

                    NotifyPropertyChanged(nameof(Items));
                }
            }
        }
        #endregion

        #region SelectedItem
        TTrackedModel __SelectedItem;
        public TTrackedModel SelectedItem
        {
            get
            {
                return this.__SelectedItem;
            }

            set
            {
                if (this.__SelectedItem != value)
                {
                    this.__SelectedItem = value;
                    NotifyPropertyChanged(nameof(SelectedItem));
                }
            }
        }
        #endregion

        #region ItemCollectionView
        ICollectionView __ItemCollectionView;
        public ICollectionView ItemCollectionView
        {
            get
            {
                return this.__ItemCollectionView;
            }

            set
            {
                if (this.__ItemCollectionView != value)
                {
                    this.__ItemCollectionView = value;
                    NotifyPropertyChanged(nameof(ItemCollectionView));
                }
            }
        }
        #endregion

        #region HasChanges
        bool __HasChanges;
        public bool HasChanges
        {
            get
            {
                return this.__HasChanges;
            }

            set
            {
                if (this.__HasChanges != value)
                {
                    this.__HasChanges = value;
                    NotifyPropertyChanged(nameof(HasChanges));
                }
            }
        }
        #endregion

        #region HasValidationMessages
        bool __HasValidationMessages;
        public bool HasValidationMessages
        {
            get
            {
                return this.__HasValidationMessages;
            }

            set
            {
                if (this.__HasValidationMessages != value)
                {
                    this.__HasValidationMessages = value;
                    NotifyPropertyChanged(nameof(HasValidationMessages));
                }
            }
        }
        #endregion

        #region IsEditing
        bool __IsEditing;
        public bool IsEditing
        {
            get
            {
                return this.__IsEditing;
            }

            set
            {
                if (this.__IsEditing != value)
                {
                    this.__IsEditing = value;
                    NotifyPropertyChanged(nameof(IsEditing));
                }
            }
        }
        #endregion

        #region AreAllGroupsExpanded
        bool __AreAllGroupsExpanded;
        public bool AreAllGroupsExpanded
        {
            get
            {
                return this.__AreAllGroupsExpanded;
            }

            set
            {
                if (this.__AreAllGroupsExpanded != value)
                {
                    this.__AreAllGroupsExpanded = value;
                    NotifyPropertyChanged(nameof(AreAllGroupsExpanded));
                }
            }
        }
        #endregion

        /// <summary>
        /// Set in derived class to support grouping.
        /// </summary>
        public string GroupPropertyName
        {
            get;
            protected set;
        }

        #region HighlightCreatedOrUpdatedItem
        bool __HighlightCreatedOrUpdatedItem;
        public bool HighlightCreatedOrUpdatedItem
        {
            get
            {
                return this.__HighlightCreatedOrUpdatedItem;
            }

            set
            {
                if (this.__HighlightCreatedOrUpdatedItem != value)
                {
                    this.__HighlightCreatedOrUpdatedItem = value;
                    NotifyPropertyChanged(nameof(HighlightCreatedOrUpdatedItem));
                }
            }
        }
        #endregion
        
        #region Abstract/Virtual members

        protected abstract TrackedListBase<TId, TTrackedModel> CreateEmptyList();

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e) { }

        protected virtual void OnCollectionItemChanged(PropertyChangedEventArgs e) { }

        #endregion

        void DataGridBindingViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IDataGridBindingViewModel.IsEditing))
            {
                UpdateCommandCanExecute();
            }

            if (e.PropertyName == nameof(this.SelectedItem))
            {
                // Update shared data service.
                this.trackedList.SetSelection(this.SelectedItem);
            }
        }

        private void ObservableCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                MultipleItemsRemovedFromCollection(e.OldItems.Cast<TTrackedModel>());
            }

            if (e.NewItems != null)
            {
                MultipleItemsAddedToCollection(e.NewItems.Cast<TTrackedModel>());
            }

            // Update shared data service.
            TrackedListUtilities.ReplayChanges<TTrackedModel, TTrackedModel>(
                e,
                trackedList,
                (sourceModel, targetModel) => sourceModel.Id.Equals(targetModel.Id),
                vm => vm);

            OnCollectionChanged(e);
            this.CollectionChanged?.Invoke();
        }

        void ITrackedList_CollectionChanged(object sender, TrackedListChangedEventArgs<TId, TTrackedModel> e)
        {
            if (e.ChangeEvent == TrackedListChangeEvent.Select)
            {
                // Update selection on grid control.
                var trackedListSelectedItem = e.NewItems?.FirstOrDefault();
                if (trackedListSelectedItem == null)
                {
                    if (this.SelectedItem != null)
                    {
                        this.SelectedItem = null;
                    }
                }
                else
                {
                    if (this.SelectedItem == null || !this.SelectedItem.Id.Equals(trackedListSelectedItem.Id))
                    {
                        var newStartingIndex = e.NewStartingIndex;
                        if (newStartingIndex > 0 && newStartingIndex < this.Items.Count)
                        {
                            this.SelectedItem = this.Items[newStartingIndex];
                        }
                    }
                }
            }
            else
            {
                // Update observable collection, that is bound to grid, based on changes to shared data service tracked list.
                var notifyCollectionChangedEventArgs = e.ConvertToNotifyCollectionChangedEventArgs();
                if (notifyCollectionChangedEventArgs != null)
                {
                    TrackedListUtilities.ReplayChanges<TTrackedModel, TTrackedModel>(
                        notifyCollectionChangedEventArgs,
                        this.Items,
                        (sourceModel, targetModel) => sourceModel.Id.Equals(targetModel.Id),
                        tm => tm);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="trackChanges">If true then the HasChanges and HasError flags are updated.</param>
        void MultipleItemsAddedToCollection(IEnumerable<TTrackedModel> items, bool trackChanges = true)
        {
            if (items == null || items.Count() == 0) return;

            foreach (var item in items) item.AddWeakEventHandler(Item_PropertyChanged);

            if (trackChanges)
            {
                UpdateHasChanges();

                // Check all items to determine if there is an error.
                UpdateHasValidationMessages();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="trackChanges">If true then the HasChanges and HasError flags are updated.</param>
        void MultipleItemsRemovedFromCollection(IEnumerable<TTrackedModel> items, bool trackChanges = true)
        {
            if (items == null || items.Count() == 0) return;

            foreach (var item in items) item.RemoveWeakEventHandler(Item_PropertyChanged);

            if (trackChanges)
            {
                UpdateHasChanges();

                // Check all items to determine if there is an error.
                UpdateHasValidationMessages();
            }
        }

        void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IModelMetadata.IsCreated) || e.PropertyName == nameof(IModelMetadata.IsEdited))
            {
                UpdateHasChanges();
            }

            if (e.PropertyName == nameof(IModelMetadata.HasValidationMessages))
            {
                UpdateHasValidationMessages();
            }

            if (e.PropertyName == nameof(IModelMetadata.IsEditing))
            {
                if ((sender is IModelMetadata itemVm))
                {
                    this.IsEditing = itemVm.IsEditing;
                }
            }

            OnCollectionItemChanged(e);
        }

        void UpdateHasChanges()
        {
            bool? anyItemHasChanges = this.Items?.Any(item => item.ModelMetadata.IsCreated || item.ModelMetadata.IsEdited);
            var hasChanges =
                (anyItemHasChanges.HasValue && anyItemHasChanges.Value) ||
                (this.RemovedItems != null && this.RemovedItems.Count > 0);
            this.HasChanges = hasChanges;
        }

        void UpdateHasValidationMessages()
        {
            bool? anyItemHasError = this.Items?.Any(item => item.ModelMetadata.HasValidationMessages);
            this.HasValidationMessages = anyItemHasError.HasValue && anyItemHasError.Value;
        }

        void ShowListView()
        {
            this.ItemCollectionView.GroupDescriptions.Clear();

            UpdateCommandCanExecute();
        }

        void ShowGroupView()
        {
            if (string.IsNullOrWhiteSpace(this.GroupPropertyName)) return;

            this.ItemCollectionView.GroupDescriptions.Clear();
            this.ItemCollectionView.GroupDescriptions.Add(new PropertyGroupDescription(this.GroupPropertyName));

            UpdateCommandCanExecute();
        }

        void ToggleGroupExpansion()
        {
            if (string.IsNullOrWhiteSpace(this.GroupPropertyName)) return;

            this.AreAllGroupsExpanded = !this.AreAllGroupsExpanded;
        }

        void UpdateCommandCanExecute()
        {
            if (this.IsEditing)
            {
                this.ShowListCommand.SetCanExecute(false);
                this.ShowGroupCommand.SetCanExecute(false);
                this.ToggleGroupExpansionCommand.SetCanExecute(false);
            }
            else
            {
                var grouped = this.ItemCollectionView.GroupDescriptions.Count > 0;

                this.ShowListCommand.SetCanExecute(grouped);
                this.ShowGroupCommand.SetCanExecute(!grouped);
                this.ToggleGroupExpansionCommand.SetCanExecute(grouped);
            }
        }
    }
}
