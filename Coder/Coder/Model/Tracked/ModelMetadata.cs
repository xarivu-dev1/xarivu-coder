using Xarivu.Coder.Model.Tracked.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Xarivu.Coder.Model.Tracked
{
    public class ModelMetadata<TId, TTrackedModel> : NotifyChangeBase, IModelMetadata
        where TId : struct
        where TTrackedModel : ITrackedModelBase<TId>
    {
        public ModelMetadata(bool isCreated, TTrackedModel trackedModel)
        {
            if (trackedModel == null) throw new ArgumentException(nameof(trackedModel));

            this.IsCreated = isCreated;
            this.TrackedModel = trackedModel;

            UpdateValidationMessages();

            this.TrackedModel.PropertyChanged += TrackedModel_PropertyChanged;
        }

        public TTrackedModel TrackedModel { get; private set; }

        #region IsCreated
        bool __IsCreated;
        public bool IsCreated
        {
            get
            {
                return this.__IsCreated;
            }

            set
            {
                if (this.__IsCreated != value)
                {
                    this.__IsCreated = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region IsUpdated
        bool __IsUpdated;
        public bool IsUpdated
        {
            get
            {
                return this.__IsUpdated;
            }

            set
            {
                if (this.__IsUpdated != value)
                {
                    this.__IsUpdated = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region IsEditing
        bool __IsEditing;
        public bool IsEditing
        {
            get
            {
                return this.__IsEditing;
            }

            set
            {
                if (this.__IsEditing != value)
                {
                    this.__IsEditing = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        /// <summary>
        /// Use to track changes while editing. In EndEdit this value is checked to see if
        /// user made any changes or clicked-out of edit mode.
        /// </summary>
        #region IsUpdatedWhileEditing
        bool __IsUpdatedWhileEditing;
        public bool IsUpdatedWhileEditing
        {
            get
            {
                return this.__IsUpdatedWhileEditing;
            }

            set
            {
                if (this.__IsUpdatedWhileEditing != value)
                {
                    this.__IsUpdatedWhileEditing = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region IsEdited
        bool __IsEdited;
        public bool IsEdited
        {
            get
            {
                return this.__IsEdited;
            }

            set
            {
                if (this.__IsEdited != value)
                {
                    this.__IsEdited = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region HasValidationMessages
        bool __HasValidationMessages;
        public bool HasValidationMessages
        {
            get
            {
                return this.__HasValidationMessages;
            }

            set
            {
                if (this.__HasValidationMessages != value)
                {
                    this.__HasValidationMessages = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region ValidationMessages
        ObservableCollection<TrackedModelValidationMessage> __ValidationMessages;
        public ObservableCollection<TrackedModelValidationMessage> ValidationMessages
        {
            get
            {
                return this.__ValidationMessages;
            }

            set
            {
                if (this.__ValidationMessages != value)
                {
                    this.__ValidationMessages = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        void TrackedModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Wait for final change and then take action.
            var changeEventArgs = e as ExtendedPropertyChangedEventArgs;
            if (changeEventArgs != null && changeEventArgs.IsFinal)
            {
                // Run with delayed notification so listener can use ExtendedPropertyChangedEventArgs
                // IsFinal flag to determine when all changes have been notified.
                RunWithDifferentNotification(
                    PropertyChangedNotificationType.Delayed,
                    () =>
                    {
                        // Add a notification to indicate the underlying tracked-model changed.
                        NotifyPropertyChanged(nameof(TrackedModel));

                        this.IsUpdated = true;

                        if (this.IsEditing)
                        {
                            this.IsUpdatedWhileEditing = true;
                        }

                        UpdateValidationMessages();
                    });
            }
        }

        void UpdateValidationMessages()
        {
            bool hasDifferentMessages = this.TrackedModel.Validate(this.ValidationMessages, out List<TrackedModelValidationMessage> newMessages);
            if (hasDifferentMessages)
            {
                this.ValidationMessages = new ObservableCollection<TrackedModelValidationMessage>(newMessages);

                this.HasValidationMessages = this.ValidationMessages != null && this.ValidationMessages.Count > 0;
            }
        }

        #region IEditableObject member implementation

        void IEditableObject.BeginEdit()
        {
            if (!this.IsEditing)
            {
                this.IsEditing = true;
                this.IsUpdatedWhileEditing = false;
            }
        }

        void IEditableObject.CancelEdit()
        {
            if (this.IsEditing)
            {
                this.IsEditing = false;
                this.IsUpdatedWhileEditing = false;
            }
        }

        void IEditableObject.EndEdit()
        {
            if (this.IsEditing)
            {
                this.IsEditing = false;

                if (this.IsUpdatedWhileEditing)
                {
                    this.IsUpdatedWhileEditing = false;

                    if (!this.IsCreated)
                    {
                        this.IsEdited = true;
                    }
                }
            }
        }

        #endregion IEditableObject member implementation
    }
}
