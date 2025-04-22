using Google.Cloud.Firestore;
using CarsHistory.Items;
using static System.Diagnostics.Debug;

namespace CarsHistory.Converter;

public class FieldWithAuthorConverter<T> : IFirestoreConverter<FieldWithAuthor<T>>
{
    public FieldWithAuthor<T> FromFirestore(object value)
    {
        if (value is IDictionary<string, object> dictionary)
        {
            if (dictionary.TryGetValue("fieldValue", out var fieldValueObject))
            {
                T fieldValue = default;

                if (fieldValueObject is Timestamp timestamp)
                {
                    if (typeof(T) == typeof(DateTime?) || typeof(T) == typeof(DateTime))
                    {
                        fieldValue = (T)(object)timestamp.ToDateTime();
                    }
                    else
                    {
                        WriteLine($"Warning: Unexpected Timestamp for type {typeof(T)}");
                        return null;
                    }
                }
                else
                {
                    try
                    {
                        fieldValue = (T)fieldValueObject;
                    }
                    catch (InvalidCastException ex)
                    {
                        WriteLine($"Error casting {fieldValueObject?.GetType()} to {typeof(T)}: {ex.Message}");
                        return null;
                    }
                }

                if (dictionary.TryGetValue("lastPersonChange", out var lastPersonChangeObject))
                {
                    string lastPersonChange = lastPersonChangeObject?.ToString() ?? string.Empty;
                    return new FieldWithAuthor<T>
                    {
                        fieldValue = fieldValue,
                        lastPersonChange = lastPersonChange
                    };
                }
                else
                {
                    WriteLine("Warning: 'lastPersonChange' not found in Firestore data.");
                    return new FieldWithAuthor<T> { fieldValue = fieldValue, lastPersonChange = string.Empty };
                }
            }
            else
            {
                WriteLine("Warning: 'fieldValue' not found in Firestore data.");
                return null;
            }
        }
        else
        {
            WriteLine($"Warning: Expected IDictionary, but got {value?.GetType()}.");
            return null;
        }
    }

    public object ToFirestore(FieldWithAuthor<T> value)
    {
        if (value == null)
        {
            return null;
        }

        return new Dictionary<string, object>
        {
            { "fieldValue", value.fieldValue },
            { "lastPersonChange", value.lastPersonChange }
        };
    }
}