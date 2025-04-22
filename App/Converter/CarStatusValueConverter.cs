using System.Globalization;
using System.Windows;
using System.Windows.Data;
using CarsHistory.Items;
using CarsHistory.Services;

namespace CarsHistory.Converter;

public class CarStatusValueConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is FieldWithAuthor<DateTime?> { fieldValue: not null } fieldWithAuthor) 
            return fieldWithAuthor.fieldValue;

        return "";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string data || string.IsNullOrWhiteSpace(data)) 
            return DependencyProperty.UnsetValue;

        try
        {
            var date = DateTime.ParseExact(data, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                
            var fieldWithAuthor = new FieldWithAuthor<DateTime?>()
            {
                fieldValue = date.ToUniversalTime(),
                lastPersonChange = FirebaseService.GetInstance().CurUserName
            };

            return fieldWithAuthor;
        }
        catch (FormatException)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}