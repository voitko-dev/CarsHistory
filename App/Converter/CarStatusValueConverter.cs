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
        {
            DateTime localDateTime = fieldWithAuthor.fieldValue.Value.ToLocalTime();
            return localDateTime;
        }

        return "";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string data || string.IsNullOrWhiteSpace(data)) 
            return DependencyProperty.UnsetValue;

        try
        {
            DateTime date;
            if (data.Contains('/'))
                //date = DateTime.ParseExact(data, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
                date = DateTime.UtcNow;
            else
                date = DateTime.ParseExact(data, "dd.MM.yyyy", CultureInfo.InvariantCulture);

            
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