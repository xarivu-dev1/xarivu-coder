using System;
using System.Collections.Generic;

namespace Xarivu.Coder.Model.Tracked.Interface
{
    public interface ITrackedListBase<TId, TTrackedModel> : IList<TTrackedModel>, IEnumerable<TTrackedModel>
        where TId : struct
        where TTrackedModel : class, ITrackedModelBase<TId>, new()
    {
        TTrackedModel SelectedItem { get; }

        bool Replace(TId replaceId, TTrackedModel model);
        bool Remove(TId removeId);
        void Move(int oldIndex, int newIndex);

        void SetSelection(TTrackedModel model);

        List<TTrackedModel> ToTrackedModelList();

        //void AddWeakEventHandler(EventHandler<TrackedListChangedEventArgs<TId, TTrackedModel>> eventHandler);
        //void RemoveWeakEventHandler(EventHandler<TrackedListChangedEventArgs<TId, TTrackedModel>> eventHandler);
    }
}
