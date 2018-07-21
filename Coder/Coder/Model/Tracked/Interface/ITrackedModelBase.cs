using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Xarivu.Coder.Model.Tracked.Interface
{
    public interface ITrackedModelBase<TId> : INotifyPropertyChanged
        where TId : struct
    {
        TId Id { get; }
        IModelMetadata ModelMetadata { get; }
        bool Validate(IList<TrackedModelValidationMessage> currentMessages, out List<TrackedModelValidationMessage> newMessages);
        void AddWeakEventHandler(EventHandler<PropertyChangedEventArgs> eventHandler);
        void RemoveWeakEventHandler(EventHandler<PropertyChangedEventArgs> eventHandler);
    }
}
