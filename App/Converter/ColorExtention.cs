using System.Drawing;

namespace CarsHistory.Converter;

public static class ColorExtention
{
    public static string ToHex(this Color color)
    {
        return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
    }
}