﻿using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CarsHistory.Converter.UserManagment;

public class BoolToBlockButtonColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isBlocked)
        {
            return isBlocked ? Brushes.LightGreen : Brushes.LightCoral;
        }
            
        return Brushes.LightGray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
