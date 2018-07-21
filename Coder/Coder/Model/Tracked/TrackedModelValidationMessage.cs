namespace Xarivu.Coder.Model.Tracked
{
    public class TrackedModelValidationMessage
    {
        public TrackedModelValidationMessage(string message)
        {
            this.Message = message;
        }

        public string Message { get; private set; }

        public override bool Equals(object obj)
        {
            var message = obj as TrackedModelValidationMessage;
            if (message == null) return false;

            if (message.Message != this.Message) return false;

            return true;
        }

        public override int GetHashCode()
        {
            return this.Message.GetHashCode();
        }
    }
}
