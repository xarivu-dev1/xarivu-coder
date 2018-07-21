using Xarivu.Coder.ViewModel;
using Xarivu.Coder.Utilities;
using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace Xarivu.Coder.View
{
    /// <summary>
    /// Interaction logic for NotificationControl.xaml
    /// </summary>
    public partial class NotificationControl : UserControl
    {
        public NotificationControl()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                this.DataContext = DependencyContainer.Get<NotificationViewModel>();
            }
        }

        private void NotificationTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                TextBox tb = sender as TextBox;
                tb.ScrollToEnd();
            }));
        }
    }
}
