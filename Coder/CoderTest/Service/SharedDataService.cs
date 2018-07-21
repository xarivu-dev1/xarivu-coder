using Xarivu.CoderTest.Model;

namespace Xarivu.CoderTest.Service
{
    public class SharedDataService
    {
        public SharedDataService()
        {
            this.TestModelList = new TestTrackedList();
        }

        public TestTrackedList TestModelList { get; set; }
    }
}
