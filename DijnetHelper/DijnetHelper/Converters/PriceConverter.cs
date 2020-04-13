using System;
using System.Globalization;
using DijnetHelper.Model;
using Xamarin.Forms;

namespace DijnetHelper.Converters
{
    public class PriceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Price price = (Price)value;
            return $"{price.Value} {price.Currency}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("TODO");
        }
    }
}
