using CarsHistory.Items;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CarsHistory.Converter
{
    public class OfferStatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OfferStatus status)
            {
                switch (status)
                {
                    case OfferStatus.Playing:
                        return new SolidColorBrush(Colors.LightGreen);
                    case OfferStatus.NotPlaying:
                        return new SolidColorBrush(Colors.LightCoral);
                    case OfferStatus.Expired:
                        return new SolidColorBrush(Colors.Gray);
                    case OfferStatus.NotSelected:
                        return Brushes.Transparent;
                }
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}