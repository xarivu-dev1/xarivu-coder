using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace Xarivu.Coder.View.Converter
{
    public class DataGridItemBindingVmBackgroundConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 4 || values.Any(v => !(v is bool))) return null;

            var isCreated = (bool)values[0];
            var isEdited = (bool)values[1];
            var hasValidationMessages = (bool)values[2];
            var highlightCreatedOrUpdatedItem = (bool)values[3];

            if (hasValidationMessages) return Brushes.LightSalmon;
            if (isCreated && highlightCreatedOrUpdatedItem) return Brushes.LightSkyBlue;
            if (isEdited && highlightCreatedOrUpdatedItem) return Brushes.LightSeaGreen;

            return Brushes.White;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
