using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Xarivu.Coder.View.Control
{
    public class CoderDecimalTextBox : TextBox
    {
        string negativeSign = Thread.CurrentThread.CurrentCulture.NumberFormat.NegativeSign;
        string decimalSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

        public CoderDecimalTextBox()
        {
            this.TextChanged += DecimalTextBox_TextChanged;
        }

        #region DecimalValue Dependency Property
        public static readonly DependencyProperty DecimalValueProperty = DependencyProperty.Register(
            "DecimalValue",                 // Property name
            typeof(decimal?),               // Property type
            typeof(CoderDecimalTextBox),    // Parent control type
            new FrameworkPropertyMetadata(
                null,                       // Default value
                OnDecimalValuePropertyChanged));

        private static void OnDecimalValuePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            CoderDecimalTextBox control = source as CoderDecimalTextBox;
            if (control != null)
            {
                control.UpdateControlFromDp();
            }
        }

        public decimal? DecimalValue
        {
            get { return (decimal?)GetValue(DecimalValueProperty); }
            set { SetValue(DecimalValueProperty, value); }
        }
        #endregion

        #region IsValidValue Dependency Property
        public static readonly DependencyProperty IsValidValueProperty = DependencyProperty.Register(
            "IsValidValue",                 // Property name
            typeof(bool),                   // Property type
            typeof(CoderDecimalTextBox),    // Parent control type
            new FrameworkPropertyMetadata(
                false,                      // Default value
                OnIsValidValuePropertyChanged));

        private static void OnIsValidValuePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            CoderDecimalTextBox control = source as CoderDecimalTextBox;
            if (control != null)
            {
                control.UpdateIsValidBackgroundColor();
            }
        }

        public bool IsValidValue
        {
            get { return (bool)GetValue(IsValidValueProperty); }
            set { SetValue(IsValidValueProperty, value); }
        }
        #endregion

        #region IsNullAllowed Dependency Property
        public static readonly DependencyProperty IsNullAllowedProperty = DependencyProperty.Register(
            "IsNullAllowed",                // Property name
            typeof(bool),                   // Property type
            typeof(CoderDecimalTextBox),    // Parent control type
            new FrameworkPropertyMetadata(
                false,                      // Default value
                OnIsNullAllowedPropertyChanged));

        private static void OnIsNullAllowedPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
        }

        public bool IsNullAllowed
        {
            get { return (bool)GetValue(IsNullAllowedProperty); }
            set { SetValue(IsValidValueProperty, value); }
        }
        #endregion

        private void DecimalTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateDpFromControl();
        }

        void UpdateControlFromDp()
        {
            if (!this.DecimalValue.HasValue)
            {
                this.Text = string.Empty;
            }
            else
            {
                decimal value = this.DecimalValue.Value;

                // If the text box value is not same as the DP value then update the text box value.
                var ok = DecimalFromText(this.Text, out decimal? textBoxValue);
                if (!ok || textBoxValue != value)
                {
                    var str = value.ToString();
                    this.Text = value.ToString();
                }
            }
        }

        void UpdateDpFromControl()
        {
            // If the text box value is not same as the DP value then update the DP value.
            var ok = DecimalFromText(this.Text, out decimal? textBoxValue);
            if (ok && textBoxValue != this.DecimalValue)
            {
                this.DecimalValue = textBoxValue;
            }

            this.IsValidValue = ok;
        }

        bool DecimalFromText(string text, out decimal? value)
        {
            value = null;

            if (this.IsNullAllowed && string.IsNullOrEmpty(text)) return true;

            var ok = decimal.TryParse(text, out decimal d);
            if (!ok) return false;

            value = d;
            return true;
        }

        void UpdateIsValidBackgroundColor()
        {
            if (this.IsValidValue)
            {
                this.Background = Brushes.White;
            }
            else
            {
                this.Background = Brushes.LightSalmon;
            }
        }
    }
}
