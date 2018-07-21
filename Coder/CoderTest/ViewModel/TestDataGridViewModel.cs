using Xarivu.Coder.Model.Tracked;
using Xarivu.Coder.ViewModel.DataGridBinding;
using Xarivu.CoderTest.Model;
using System;

namespace Xarivu.CoderTest.ViewModel
{
    public class TestDataGridViewModel : DataGridBindingViewModel<Guid, TestTrackedModel>
    {
        public TestDataGridViewModel(TestTrackedList initialList)
            : base(initialList)
        {
            this.IsReadOnly = false;
            this.CanUserAddRows = true;
            this.CanUserDeleteRows = true;
        }

        protected override TrackedListBase<Guid, TestTrackedModel> CreateEmptyList()
        {
            return new TestTrackedList();
        }
    }
}
