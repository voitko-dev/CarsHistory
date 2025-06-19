using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CarsHistory.Converter.UserManagment;

public class BoolToBlockedColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isBlocked)
        {
            return isBlocked ? Brushes.Red : Brushes.Green;
        }
            
        return Brushes.Gray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
