namespace CarsHistory.Extentions;

public static class StringExtentions
{
    public static bool isNullOrEmpty(this string? str)
    {
        return string.IsNullOrEmpty(str);
    }
}