using Xarivu.Coder.Model.Tracked;
using System.Collections.Generic;

namespace Xarivu.CoderTest.Model
{
    public class TestTrackedList : TrackedListGuidBase<TestTrackedModel>
    {
        public TestTrackedList(IEnumerable<TestTrackedModel> initialList = null) : base(initialList)
        {
        }
    }
}
