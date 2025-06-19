using System.Globalization;
using System.Windows.Data;

namespace CarsHistory.Converter.UserManagment;

public class BoolToBlockedStatusConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isBlocked)
        {
            return isBlocked ? "Заблоковано" : "Активний";
        }
            
        return "Невідомо";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}