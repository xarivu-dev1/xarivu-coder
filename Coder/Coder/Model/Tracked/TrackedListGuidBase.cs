using Xarivu.Coder.Model.Tracked.Interface;
using System;
using System.Collections.Generic;

namespace Xarivu.Coder.Model.Tracked
{
    /// <summary>
    /// Base class for tracked list.
    /// Derive from this for lists that contain tracked models where the ID is GUID type.
    /// </summary>
    /// <typeparam name="TTrackedModel"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    public abstract class TrackedListGuidBase<TTrackedModel> : TrackedListBase<Guid, TTrackedModel>
        where TTrackedModel : class, ITrackedModelBase<Guid>, new()
    {
        public TrackedListGuidBase(IEnumerable<TTrackedModel> initialList = null)
            : base(initialList)
        {
        }
    }
}
