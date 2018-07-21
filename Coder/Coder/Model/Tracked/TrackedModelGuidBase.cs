using System;

namespace Xarivu.Coder.Model.Tracked
{
    /// <summary>
    /// Base class for tracked-models that don't have unique ID in TModel.
    /// This class uses a new Guid as the ID.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public abstract class TrackedModelGuidBase<TModel> : TrackedModelBase<Guid, TModel>
    {
        Guid id;

        public TrackedModelGuidBase()
        {
            this.id = Guid.NewGuid();
        }

        public TrackedModelGuidBase(TModel initialModel)
            : base(initialModel)
        {
            this.id = Guid.NewGuid();
        }

        public override Guid Id => this.id;
    }
}
