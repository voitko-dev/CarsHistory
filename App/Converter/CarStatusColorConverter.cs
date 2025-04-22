using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using CarsHistory.Items;
using CarsHistory.Services;

namespace CarsHistory.Converter;

public class CarStatusColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null || string.IsNullOrEmpty(value.ToString()))
            return "#7a2b2b";

        if (value is not FieldWithAuthor<DateTime?> fieldWithAuthor ||
            string.IsNullOrEmpty(fieldWithAuthor.lastPersonChange))
            return Brushes.Transparent;

        return UsersService.GetInstance().ColorData.TryGetValue(fieldWithAuthor.lastPersonChange, out var color)
            ? new SolidColorBrush((Color)ColorConverter.ConvertFromString(color))
            : Brushes.Transparent;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}