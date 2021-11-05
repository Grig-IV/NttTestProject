using System;
using System.Globalization;
using System.Net;
using System.Windows.Data;

namespace NttLibrary.Converters
{
    public class IpConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IPAddress result;
            IPAddress.TryParse(value as string, out result);
            return result;
        }
    }
}
