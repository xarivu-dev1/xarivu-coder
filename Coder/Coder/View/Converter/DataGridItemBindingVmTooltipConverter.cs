using Xarivu.Coder.Model.Tracked;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Xarivu.Coder.View.Converter
{
    public class DataGridItemBindingVmTooltipConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 1) return null;

            string tooltip = null;

            var validationMessages = values[0] as IEnumerable<TrackedModelValidationMessage>;
            var count = validationMessages?.Count() ?? 0;
            if (count > 0)
            {
                tooltip = validationMessages.First().Message;
                if (count > 1)
                {
                    tooltip += Environment.NewLine;
                    tooltip += $"{count - 1} more message{(count == 2 ? string.Empty : "s")}.";
                }
            }

            return tooltip;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
