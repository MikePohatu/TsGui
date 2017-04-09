using System;
using System.Globalization;
using System.Windows.Data;

namespace TsGui.View.Layout.Converters
{
    public class XmlStringLineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string valueAsString = value as string;
            if (string.IsNullOrEmpty(valueAsString)) { return value; }

            valueAsString = valueAsString.Replace("\\r", "\r");
            valueAsString = valueAsString.Replace("\\n", "\n");
            return valueAsString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
