using System;
using System.Globalization;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Xarivu.Coder.View.Control
{
    public class CoderDatePicker : DatePicker
    {
        readonly CultureInfo currentCulture;
        readonly string shortDatePattern;

        DatePickerTextBox datePickerTextBox;

        public CoderDatePicker()
            : base()
        {
            this.currentCulture = Thread.CurrentThread.CurrentCulture;
            this.shortDatePattern = this.currentCulture.DateTimeFormat.ShortDatePattern;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.datePickerTextBox = this.Template.FindName("PART_TextBox", this) as DatePickerTextBox;
            if (this.datePickerTextBox != null)
            {
                this.datePickerTextBox.TextChanged += DatePickerTextBox_TextChanged;
            }
        }

        private void DatePickerTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DateTime dt;

            var ok = DateTime.TryParseExact(
                this.datePickerTextBox.Text,
                this.shortDatePattern,
                this.currentCulture,
                DateTimeStyles.None,
                out dt);

            if (ok)
            {
                this.SelectedDate = dt;
            }
        }
    }
}
