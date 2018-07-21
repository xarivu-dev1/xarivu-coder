using Xarivu.Coder.Component.ObjectView.ViewModel;
using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace Xarivu.Coder.Component.ObjectView.Converter
{
    [ValueConversion(typeof(object), typeof(string))]
    public class DataGridTextColumnPropertyInfoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                RowDataHolder dh = value as RowDataHolder;
                if (dh == null || dh.Data == null)
                {
                    return string.Empty;
                }

                PropertyInfo propInfo = parameter as PropertyInfo;
                if (propInfo == null)
                {
                    return string.Empty;
                }

                object dv = propInfo.GetValue(dh.Data);
                if (dv == null)
                {
                    return string.Empty;
                }

                string text = string.Empty;
                var propertyType = propInfo.PropertyType;
                if (propertyType.IsValueType)
                {
                    text = dv.ToString();
                }
                else if (propertyType == typeof(string))
                {
                    text = dv as string;
                    if (text != null)
                    {
                        // Check if text contains new line and trim.
                        var newLine1Index = text.IndexOf('\r');
                        var newLine2Index = text.IndexOf('\n');
                        if (newLine1Index != -1 || newLine2Index != -1)
                        {
                            text = text.Remove(newLine1Index != -1 ? newLine1Index : newLine2Index);
                        }
                    }
                }
                else
                {
                    // Not a value type or a string. Show the name of the property in curly braces.
                    text = $"{{{propInfo.Name}}}";
                }

                return text;
            }
            catch (Exception ex)
            {
                var s = ex.ToString();
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
