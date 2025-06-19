using System.Drawing;

namespace CarsHistory.Extentions;

public static class ColorExtention
{
    public static string ToHex(this Color color)
    {
        return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
    }
}