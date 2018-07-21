using Xarivu.Coder.Model.Tracked;
using Xarivu.Coder.Model.Tracked.Interface;
using Xarivu.CoderTest.Model;
using Xarivu.CoderTest.Service;
using System;

namespace Xarivu.CoderTest.ViewModel
{
    public class DataGridTestControlViewModel : NotifyChangeBase
    {
        SharedDataService sharedDataService;

        public DataGridTestControlViewModel(SharedDataService sharedDataService)
        {
            this.sharedDataService = sharedDataService;
            this.TestDataGridViewModel = new TestDataGridViewModel(this.sharedDataService.TestModelList);
        }

        #region TestDataGridViewModel
        TestDataGridViewModel __TestDataGridViewModel;
        public TestDataGridViewModel TestDataGridViewModel
        {
            get
            {
                return this.__TestDataGridViewModel;
            }

            set
            {
                if (this.__TestDataGridViewModel != value)
                {
                    this.__TestDataGridViewModel = value;
                    NotifyPropertyChanged(nameof(TestDataGridViewModel));
                }
            }
        }
        #endregion
    }
}
