using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Xarivu.Coder.Model.Tracked
{
    public abstract class NotifyChangeBase : INotifyPropertyChanged
    {
        bool disableNotification;
        
        // Queue of ExtendedPropertyChangedEventArgs that were created when notification was disabled.
        List<ExtendedPropertyChangedEventArgs> changedWhenNotificationDisabled;

        public NotifyChangeBase()
        {
            this.PropertyChanged += NotifyChangeBase_PropertyChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void NotifyChangeBase_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChangedInternal(e as ExtendedPropertyChangedEventArgs);
        }

        protected virtual void PropertyChangedInternal(ExtendedPropertyChangedEventArgs e)
        {
        }

        protected void RunWithDifferentNotification(PropertyChangedNotificationType notificationType, Action action)
        {
            try
            {
                this.disableNotification = true;
                this.changedWhenNotificationDisabled = new List<ExtendedPropertyChangedEventArgs>();

                action();
            }
            finally
            {
                this.disableNotification = false;

                // Copy reference to list of notifications fired, and set the class member (changedWhenNotificationDisabled) to null.
                // If there is an exception in the finally block, the class member will be clean.

                var tempChangedWhenNotificationDisabled = this.changedWhenNotificationDisabled;
                this.changedWhenNotificationDisabled = null;

                // Invoke notification(s) if not disabled.
                if (notificationType == PropertyChangedNotificationType.Delayed && this.PropertyChanged != null)
                {
                    var total = tempChangedWhenNotificationDisabled.Count;
                    for (int i = 0; i < total; ++i)
                    {
                        var extendedEventArgs = tempChangedWhenNotificationDisabled[i];

                        extendedEventArgs.Total = total;
                        extendedEventArgs.Index = i;
                        extendedEventArgs.IsFinal = i == total - 1;

                        // Note that the call to PropertyChanged may lead to an event handler calling back into RunWithDifferentNotification.
                        this.PropertyChanged(this, extendedEventArgs);
                    }
                }
            }
        }

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            NotifyPropertyChanged(new ExtendedPropertyChangedEventArgs(propertyName));
        }

        protected void NotifyPropertyChanged(ExtendedPropertyChangedEventArgs eventArgs)
        {
            if (this.disableNotification)
            {
                this.changedWhenNotificationDisabled.Add(eventArgs);
            }
            else if(this.PropertyChanged != null)
            {
                this.PropertyChanged(this, eventArgs);
            }
        }

        public void AddWeakEventHandler(EventHandler<PropertyChangedEventArgs> eventHandler)
        {
            WeakEventManager<NotifyChangeBase, PropertyChangedEventArgs>.AddHandler(
                this,
                nameof(PropertyChanged),
                eventHandler);
        }

        public void RemoveWeakEventHandler(EventHandler<PropertyChangedEventArgs> eventHandler)
        {
            WeakEventManager<NotifyChangeBase, PropertyChangedEventArgs>.AddHandler(
                this,
                nameof(PropertyChanged),
                eventHandler);
        }
    }
}
