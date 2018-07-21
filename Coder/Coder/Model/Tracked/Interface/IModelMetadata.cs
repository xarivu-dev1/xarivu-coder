using System;
using System.ComponentModel;

namespace Xarivu.Coder.Model.Tracked.Interface
{
    public interface IModelMetadata : IEditableObject
    {
        bool IsCreated { get; }
        bool IsUpdated { get; }
        bool IsEditing { get; }
        bool IsEdited { get; }
        bool HasValidationMessages { get; }

        void AddWeakEventHandler(EventHandler<PropertyChangedEventArgs> eventHandler);
        void RemoveWeakEventHandler(EventHandler<PropertyChangedEventArgs> eventHandler);
    }
}
