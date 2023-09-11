namespace Squill.Data.Utility;

public class SqColor
{
    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }
    public byte A { get; set; }

    public SqColor(byte r, byte g, byte b, byte a = 255)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public SqColor(string hexColor)
    {
        if (hexColor.StartsWith("#"))
            hexColor = hexColor.Substring(1);

        if (hexColor.Length == 6)
        {
            R = Convert.ToByte(hexColor.Substring(0, 2), 16);
            G = Convert.ToByte(hexColor.Substring(2, 2), 16);
            B = Convert.ToByte(hexColor.Substring(4, 2), 16);
            A = 255;
        }
        else if (hexColor.Length == 8)
        {
            R = Convert.ToByte(hexColor.Substring(0, 2), 16);
            G = Convert.ToByte(hexColor.Substring(2, 2), 16);
            B = Convert.ToByte(hexColor.Substring(4, 2), 16);
            A = Convert.ToByte(hexColor.Substring(6, 2), 16);
        }
        else
        {
            throw new ArgumentException("Invalid hex color string.", nameof(hexColor));
        }
    }
}

public static class ColorExtensions
{
    public static string ToHexString(this SqColor color)
    {
        return $"#{color.R:X2}{color.G:X2}{color.B:X2}{color.A:X2}";
    }

    public static SqColor FromHSLA(double h, double s, double l, byte a = 255)
    {
        if (s < 0.0 || s > 1.0)
            throw new ArgumentOutOfRangeException(nameof(s), "Saturation must be between 0 and 1.");
        if (l < 0.0 || l > 1.0)
            throw new ArgumentOutOfRangeException(nameof(l), "Luminance must be between 0 and 1.");

        double c = (1 - Math.Abs(2 * l - 1)) * s;
        double x = c * (1 - Math.Abs((h / 60) % 2 - 1));
        double m = l - c / 2;

        double r, g, b;

        if (h >= 0 && h < 60)
        {
            r = c;
            g = x;
            b = 0;
        }
        else if (h >= 60 && h < 120)
        {
            r = x;
            g = c;
            b = 0;
        }
        else if (h >= 120 && h < 180)
        {
            r = 0;
            g = c;
            b = x;
        }
        else if (h >= 180 && h < 240)
        {
            r = 0;
            g = x;
            b = c;
        }
        else if (h >= 240 && h < 300)
        {
            r = x;
            g = 0;
            b = c;
        }
        else
        {
            r = c;
            g = 0;
            b = x;
        }

        byte red = (byte)((r + m) * 255);
        byte green = (byte)((g + m) * 255);
        byte blue = (byte)((b + m) * 255);

        return new SqColor(red, green, blue, a);
    }

    public static SqColor WithAlpha(this SqColor color, byte alpha)
    {
        return new SqColor(color.R, color.G, color.B, alpha);
    }
}