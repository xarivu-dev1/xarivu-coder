using System;
using System.Globalization;
using System.Windows.Data;

namespace Xarivu.Coder.View.Converter
{
    [ValueConversion(typeof(object), typeof(bool))]
    public class NullRefToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
