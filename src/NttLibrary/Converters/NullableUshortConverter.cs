using System;
using System.Globalization;
using System.Windows.Data;

namespace NttLibrary.Converters
{
    public class NullableUshortConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ushort result;
            var canConvert = ushort.TryParse(value as string, out result);
            return canConvert ? result : null;
        }
    }
}
