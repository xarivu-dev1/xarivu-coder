using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Xarivu.Coder.View.Converter
{
    /// <summary>
    /// Converter for data grid selected item. When the new-item place holder is selected, set view model selected-item to null.
    /// </summary>
    public class DataGridSelectedItemIgnorePlaceholderConverter : IValueConverter
    {
        private const string newItemPlaceholderName = "{NewItemPlaceholder}";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // When converting back to view-model, if the new item place holder is selected in data grid, then set selected-item to null.
            if (value != null && value.ToString() == newItemPlaceholderName)
            {
                value = null;
            }

            return value;
        }
    }
}
