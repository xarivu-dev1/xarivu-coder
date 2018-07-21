using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Xarivu.Coder.Model.Tracked
{
    public static class TrackedListUtilities
    {
        /// <summary>
        /// Replay changes made to a source list on a target list.
        /// The changes made to source list are captured in the eventArgs parameter.
        /// </summary>
        /// <typeparam name="TId"></typeparam>
        /// <typeparam name="TSourceModel"></typeparam>
        /// <typeparam name="TTargetModel"></typeparam>
        /// <typeparam name="TTargetModel"></typeparam>
        /// <param name="sourceListEventArgs"></param>
        /// <param name="targetList"></param>
        /// <param name="targetListHasSameModelFunc"></param>
        /// <param name="createTrackedModelFromSourceModelFunc"></param>
        public static void ReplayChanges<TSourceModel, TTargetModel>(
            NotifyCollectionChangedEventArgs sourceListEventArgs,
            IList<TTargetModel> targetList,
            Func<TSourceModel, TTargetModel, bool> areSameRepresentationsFunc,
            Func<TSourceModel, TTargetModel> createTargetModelFromSourceModelFunc)
            where TSourceModel : class
            where TTargetModel : class
        {
            switch (sourceListEventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (sourceListEventArgs.NewItems != null)
                    {
                        var index = sourceListEventArgs.NewStartingIndex;
                        var hasSameModel = HasSameRepresentation(
                            sourceListEventArgs.NewItems[0] as TSourceModel,
                            targetList,
                            index,
                            areSameRepresentationsFunc);
                            
                        if (!hasSameModel)
                        {
                            foreach (TSourceModel sourceModel in sourceListEventArgs.NewItems)
                            {
                                var targetModel = createTargetModelFromSourceModelFunc(sourceModel);
                                targetList.Insert(index, targetModel);

                                ++index;
                            }
                        }
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (sourceListEventArgs.OldItems != null)
                    {
                        var index = sourceListEventArgs.OldStartingIndex;

                        var hasSameModel = HasSameRepresentation(
                            sourceListEventArgs.OldItems[0] as TSourceModel,
                            targetList,
                            index,
                            areSameRepresentationsFunc);

                        if (hasSameModel)
                        {
                            foreach (var trackedModel in sourceListEventArgs.OldItems)
                            {
                                targetList.RemoveAt(index);
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (sourceListEventArgs.OldItems != null)
                    {
                        var index = sourceListEventArgs.OldStartingIndex;
                        
                        var hasSameModel = HasSameRepresentation(
                            sourceListEventArgs.OldItems[0] as TSourceModel,
                            targetList,
                            index,
                            areSameRepresentationsFunc);

                        if (hasSameModel)
                        {
                            foreach (TSourceModel sourceModel in sourceListEventArgs.NewItems)
                            {
                                var targetModel = createTargetModelFromSourceModelFunc(sourceModel);
                                targetList[index] = targetModel;
                                ++index;
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    var oldIndex = sourceListEventArgs.OldStartingIndex;

                    var hasSameModelAtOldIndex = HasSameRepresentation(
                            sourceListEventArgs.OldItems[0] as TSourceModel,
                            targetList,
                            oldIndex,
                            areSameRepresentationsFunc);

                    if (hasSameModelAtOldIndex)
                    {
                        var newIndex = sourceListEventArgs.NewStartingIndex;
                        var numItemsToMove = sourceListEventArgs.NewItems?.Count ?? 1;
                        var lastOldIndex = oldIndex + numItemsToMove - 1;
                        var lastNewIndex = newIndex + numItemsToMove - 1;
                        if (lastNewIndex >= targetList.Count)
                        {
                            // Cannot move.
                        }
                        else
                        {
                            // Get all items to move.
                            var itemsToMove = new List<TTargetModel>();
                            for (int i = oldIndex; i <= lastOldIndex; ++i)
                            {
                                itemsToMove.Add(targetList[i]);
                            }

                            for (int i = oldIndex; i <= lastOldIndex; ++i)
                            {
                                targetList.RemoveAt(oldIndex);
                            }

                            int j = 0;
                            for (int i = newIndex; i <= lastNewIndex; ++i)
                            {
                                targetList.Insert(i, itemsToMove[j++]);
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    if (targetList.Count > 0)
                    {
                        targetList.Clear();
                    }
                    break;
            }
        }

        public static bool HasSameRepresentation<TSourceModel, TTargetModel>(
            TSourceModel sourceModel,
            IList<TTargetModel> targetList,
            int index,
            Func<TSourceModel, TTargetModel, bool> areSameRepresentationsFunc)
            where TSourceModel : class
            where TTargetModel : class
        {
            if (index < 0 || index >= targetList.Count) return false;

            var targetModel = targetList[index];
            return areSameRepresentationsFunc(sourceModel, targetModel);
        }
    }
}
