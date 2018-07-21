using System.ComponentModel;

namespace Xarivu.Coder.Model.Tracked
{
    public class ExtendedPropertyChangedEventArgs : PropertyChangedEventArgs
    {
        object originalValue;

        public ExtendedPropertyChangedEventArgs(string propertyName) : base(propertyName)
        {
            Total = 1;
            Index = 0;
            IsFinal = true;
        }

        public ExtendedPropertyChangedEventArgs(string propertyName, object originalValue = null)
            : this(propertyName)
        {
            this.originalValue = originalValue;
        }

        public int Total { get; set; }
        public int Index { get; set; }

        /// <summary>
        /// True if this is the final notification sent as part of a sequence of notifications
        /// or if this is not part of a sequence.
        /// </summary>
        public bool IsFinal { get; set; }

        public T GetOriginalValue<T>()
        {
            return (T)this.originalValue;
        }
    }
}
