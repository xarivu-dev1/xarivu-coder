using Xarivu.Coder.Model.Tracked.Interface;
using System.Collections.Generic;
using System.ComponentModel;

namespace Xarivu.Coder.Model.Tracked
{
    public abstract class TrackedModelBase<TId, TModel> : NotifyChangeBase, ITrackedModelBase<TId>, IEditableObject
        where TId : struct
    {
        /// <summary>
        /// Initialize a new instance of TrackedModelBase.
        /// Call this constructor when a new model is created in the application.
        /// The model metadata IsCreated flag will be true.
        /// </summary>
        public TrackedModelBase()
        {
            Initialize(true, default(TModel));
        }

        /// <summary>
        /// Initialize a new instance of TrackedModelBase.
        /// Call this constructor when an existing model is added to the application.
        /// The model metadata IsCreated flag will be false.
        /// </summary>
        public TrackedModelBase(TModel initialModel)
        {
            Initialize(false, initialModel);
        }

        public TModel InitialModel { get; private set; }

        public IModelMetadata ModelMetadata { get; set; }

        #region Abstract/Virtual members

        public abstract TId Id { get; }

        /// <summary>
        /// Override in the derived class and initialize the tracked model based on the model.
        /// Note that model may be default(TModel), which would be null if TModel is a class, if the default constructor was used.
        /// So the code should check if model is null first and do default initialization.
        /// Default initilization (when model is null) should include instantiation of child tracked models and lists.
        /// This will only be called on initialization.
        /// </summary>
        /// <param name="model"></param>
        protected abstract void InitializeInternal(TModel model);

        protected abstract TModel ToModelInternal();

        protected virtual void ValidateInternal(List<TrackedModelValidationMessage> messages) { }

        #endregion Abstract/Virtual methods
        
        void Initialize(bool isCreated, TModel initialModel)
        {
            this.ModelMetadata = new ModelMetadata<TId, TrackedModelBase<TId, TModel>>(isCreated, this);
            this.InitialModel = initialModel;

            InitializeInternal(this.InitialModel);
        }

        /// <summary>
        /// Validates the model properties and returns new validation messages, if any.
        /// If there is no differences between the current validation messages and new validation messages,
        /// then false is returned. Otherwise true is returned.
        /// </summary>
        /// <param name="currentMessages"></param>
        /// <returns>True if there is a difference between the current and new validation messages.</returns>
        public bool Validate(IList<TrackedModelValidationMessage> currentMessages, out List<TrackedModelValidationMessage> newMessages)
        {
            newMessages = new List<TrackedModelValidationMessage>();
            ValidateInternal(newMessages);

            bool hasOldValidationMessages = currentMessages != null && currentMessages.Count > 0;

            if (newMessages.Count == 0)
            {
                // If there are no new validation messages and no old validation messages, then return false
                // to indicate no work required.
                if (!hasOldValidationMessages) return false;

                // If there are no new validation messages and there were old validation messages, then return true
                // to indicate the messages were cleared.
                if (hasOldValidationMessages)
                {
                    return true;
                }
            }

            // There are new validation messages, check if those are the same as the old validation messages.

            if (!hasOldValidationMessages)
            {
                return true;
            }

            if (newMessages.Count != currentMessages.Count)
            {
                // Counts are different.
                return true;
            }

            for (int i = 0; i < newMessages.Count; ++i)
            {
                var newMessage = newMessages[i];
                if (!newMessage.Equals(currentMessages[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public TModel ToModel()
        {
            return ToModelInternal();
        }

        #region IEditableObject member implementation

        void IEditableObject.BeginEdit()
        {
            this.ModelMetadata.BeginEdit();
        }

        void IEditableObject.CancelEdit()
        {
            this.ModelMetadata.CancelEdit();
        }

        void IEditableObject.EndEdit()
        {
            this.ModelMetadata.EndEdit();
        }

        #endregion IEditableObject member implementation
    }
}

