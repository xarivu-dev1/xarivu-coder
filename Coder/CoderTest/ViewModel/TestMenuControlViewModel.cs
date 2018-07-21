using Xarivu.Coder.Model.Tracked;
using Xarivu.Coder.ViewModel;
using Xarivu.CoderTest.Model;
using Xarivu.CoderTest.Service;

namespace Xarivu.CoderTest.ViewModel
{
    public class TestMenuControlViewModel : NotifyChangeBase
    {
        SharedDataService sharedDataService;

        public TestMenuControlViewModel(SharedDataService sharedDataService)
        {
            this.sharedDataService = sharedDataService;
            this.PopulateCommand = new DelegateCommand(p => PopulateAction());
        }

        public DelegateCommand PopulateCommand { get; private set; }

        void PopulateAction()
        {
            var testModel = new TestModel() { Name = "Temp", Value = "Val1" };

            this.sharedDataService.TestModelList.Add(new TestTrackedModel(testModel));
        }
    }
}
