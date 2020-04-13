using System;
using System.Collections;
using System.Globalization;
using Xamarin.Forms;

namespace DijnetHelper.Converters
{
    public class NotEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                return !string.IsNullOrEmpty(str);
            }

            if (value is ICollection collection)
            {
                return collection.Count > 0;
            }

            return value != default;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("TODO");
        }
    }
}
