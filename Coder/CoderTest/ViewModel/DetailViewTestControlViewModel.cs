using Xarivu.Coder.Model.Tracked;
using Xarivu.CoderTest.Model;
using Xarivu.CoderTest.Service;
using System;

namespace Xarivu.CoderTest.ViewModel
{
    public class DetailViewTestControlViewModel : NotifyChangeBase
    {
        SharedDataService sharedDataService;

        public DetailViewTestControlViewModel(SharedDataService sharedDataService)
        {
            this.sharedDataService = sharedDataService;

            this.sharedDataService.TestModelList.AddWeakEventHandler(ITrackedList_CollectionChanged);

            UpdateAll();
        }

        #region TestTrackedModel
        TestTrackedModel __TestTrackedModel;
        public TestTrackedModel TestTrackedModel
        {
            get
            {
                return this.__TestTrackedModel;
            }

            set
            {
                if (this.__TestTrackedModel != value)
                {
                    this.__TestTrackedModel = value;
                    NotifyPropertyChanged(nameof(TestTrackedModel));
                }
            }
        }
        #endregion

        #region CanEdit
        bool __CanEdit;
        public bool CanEdit
        {
            get
            {
                return this.__CanEdit;
            }

            set
            {
                if (this.__CanEdit != value)
                {
                    this.__CanEdit = value;
                    NotifyPropertyChanged(nameof(CanEdit));
                }
            }
        }
        #endregion

        void ITrackedList_CollectionChanged(object sender, TrackedListChangedEventArgs<Guid, TestTrackedModel> e)
        {
            if (e.ChangeEvent == TrackedListChangeEvent.Select)
            {
                UpdateAll();
            }
        }
        
        void UpdateAll()
        {
            UpdateTestTrackedModel();
            UpdateCanEdit();
        }

        void UpdateTestTrackedModel()
        {
            this.TestTrackedModel = this.sharedDataService.TestModelList.SelectedItem;
        }

        void UpdateCanEdit()
        {
            this.CanEdit = this.TestTrackedModel != null;
        }
    }
}
