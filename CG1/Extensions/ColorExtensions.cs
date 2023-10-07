using System.Windows.Media;

namespace CG1.Extensions;

public static class ColorExtensions
{
    public static Color ChangeColor(this Color color, short tA, short tR, short tG, short tB)
    {
        var a = color.A + tA;
        if (a < 0) a = 0;
        else if (a > 255) a = 255;
        var r = color.R + tR;
        if (r < 0) r = 0;
        else if (r > 255) r = 255;
        var g = color.G + tG;
        if (g < 0) g = 0;
        else if (g > 255) g = 255;
        var b = color.B + tB;
        if (b < 0) b = 0;
        else if (b > 255) b = 255;

        return Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);
    }
}