using Xarivu.Coder.Model.Tracked.Interface;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Xarivu.Coder.Model.Tracked
{
    public class TrackedListChangedEventArgs<TId, TTrackedModel> : ExtendedPropertyChangedEventArgs
        where TId : struct
        where TTrackedModel : class, ITrackedModelBase<TId>, new()
    {
        public TrackedListChangedEventArgs()
            : base("TrackedList")
        {
        }

        public TrackedListChangeEvent ChangeEvent { get; protected set; }

        public List<TTrackedModel> NewItems { get; protected set; }
        public List<TTrackedModel> OldItems { get; protected set; }
        public int NewStartingIndex { get; protected set; }
        public int OldStartingIndex { get; protected set; }

        public ExtendedPropertyChangedEventArgs ItemChangedEventArgs { get; protected set; }

        /// <summary>
        /// Creates a new NotifyCollectionChangedEventArgs instance based on this (TrackedListChangedEventArgs instance).
        /// Note, some change events captured by TrackedListChangedEventArgs cannot be represented as NotifyCollectionChangedEventArgs,
        /// i.e. Select or ItemChange, in those cases null is returned.
        /// </summary>
        /// <returns></returns>
        public NotifyCollectionChangedEventArgs ConvertToNotifyCollectionChangedEventArgs()
        {
            switch (this.ChangeEvent)
            {
                case TrackedListChangeEvent.Add: return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, this.NewItems, this.NewStartingIndex);
                case TrackedListChangeEvent.Move: return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, this.NewItems, this.NewStartingIndex, this.OldStartingIndex);
                case TrackedListChangeEvent.Remove: return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, this.OldItems, this.OldStartingIndex);
                case TrackedListChangeEvent.Replace: return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, this.NewItems, this.OldItems, this.NewStartingIndex);
                case TrackedListChangeEvent.Reset: return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            }

            return null;
        }
    }
}
