namespace CarsHistory.Extentions;

public static class StringExtentions
{
    public static bool IsNullOrEmpty(this string? str)
    {
        return string.IsNullOrEmpty(str);
    }
}