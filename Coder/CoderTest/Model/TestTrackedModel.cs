using System.Collections.Generic;
using Xarivu.Coder.Model.Tracked;

namespace Xarivu.CoderTest.Model
{
    public class TestTrackedModel : TrackedModelGuidBase<TestModel>
    {
        public TestTrackedModel()
        {
        }

        public TestTrackedModel(TestModel testModel)
            : base(testModel)
        {
        }

        #region Name
        string __Name;
        public string Name
        {
            get
            {
                return this.__Name;
            }

            set
            {
                if (this.__Name != value)
                {
                    this.__Name = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region Value
        string __Value;
        public string Value
        {
            get
            {
                return this.__Value;
            }

            set
            {
                if (this.__Value != value)
                {
                    this.__Value = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion
            

        protected override void InitializeInternal(TestModel model)
        {
            if (model != null)
            {
                this.Name = model.Name;
                this.Value = model.Value;
            }
        }

        protected override TestModel ToModelInternal()
        {
            return new TestModel()
            {
                Name = this.Name,
                Value = this.Value
            };
        }

        protected override void ValidateInternal(List<TrackedModelValidationMessage> messages)
        {
            if (string.IsNullOrWhiteSpace(Name)) messages.Add(new TrackedModelValidationMessage("Name is invalid."));
        }
    }
}
