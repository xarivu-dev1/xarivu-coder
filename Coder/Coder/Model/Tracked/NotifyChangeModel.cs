namespace Xarivu.Coder.Model.Tracked
{
    public abstract class NotifyChangeModel<TId, TModel> : NotifyChangeBase
        where TId : struct
    {
        public NotifyChangeModel()
        {
        }

        public NotifyChangeModel(TModel initialModel)
            : this()
        {
            this.InitialModel = initialModel;
            FromModel(initialModel);
        }

        public TModel InitialModel { get; private set; }

        #region Abstract/Virtual members

        public abstract TId Id { get; }

        protected abstract void FromModelInternal(TModel model);
        protected abstract TModel ToModelInternal();

        #endregion Abstract/Virtual methods

        public TModel ToModel()
        {
            return ToModelInternal();
        }

        /// <summary>
        /// Populate this class properties from TModel object.
        /// </summary>
        /// <param name="model"></param>
        void FromModel(TModel model)
        {
            RunWithDifferentNotification(
                PropertyChangedNotificationType.Disabled,
                () => FromModelInternal(model));
        }
    }
}
