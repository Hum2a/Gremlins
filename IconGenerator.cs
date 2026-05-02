using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Gremlins;

/// <summary>
/// Generates a simple gremlin face as a System.Drawing.Icon at runtime,
/// so we don't need an external .ico asset.
/// </summary>
public static class IconGenerator
{
    public static System.Drawing.Icon CreateGremlinIcon()
    {
        using var bmp = new Bitmap(32, 32, PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.Transparent);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Head — dark green
            g.FillEllipse(Brushes.DarkGreen, 4, 6, 24, 22);

            // Eyes — glowing yellow
            g.FillEllipse(Brushes.Yellow, 8, 11, 6, 6);
            g.FillEllipse(Brushes.Yellow, 18, 11, 6, 6);

            // Pupils
            g.FillEllipse(Brushes.Black, 10, 13, 3, 3);
            g.FillEllipse(Brushes.Black, 20, 13, 3, 3);

            // Grin
            using var pen = new Pen(Color.Black, 1.5f);
            g.DrawArc(pen, 9, 18, 14, 7, 0, 180);

            // Ears / horns
            g.FillPolygon(Brushes.DarkGreen,
                new PointF[] { new(4, 10), new(0, 2), new(9, 7) });
            g.FillPolygon(Brushes.DarkGreen,
                new PointF[] { new(28, 10), new(32, 2), new(23, 7) });
        }

        // Convert Bitmap to Icon
        using var ms = new MemoryStream();
        bmp.Save(ms, ImageFormat.Png);
        ms.Position = 0;

        // Write a minimal .ico container around the PNG
        using var iconMs = new MemoryStream();
        using var writer = new BinaryWriter(iconMs);

        // ICONDIRENTRY
        writer.Write((short)0);  // reserved
        writer.Write((short)1);  // type: icon
        writer.Write((short)1);  // count

        var pngBytes = ms.ToArray();

        writer.Write((byte)32); // width
        writer.Write((byte)32); // height
        writer.Write((byte)0);  // colour count
        writer.Write((byte)0);  // reserved
        writer.Write((short)1); // planes
        writer.Write((short)32); // bit count
        writer.Write(pngBytes.Length);
        writer.Write(22); // offset: 6 header + 16 entry

        writer.Write(pngBytes);

        iconMs.Position = 0;
        return new System.Drawing.Icon(iconMs);
    }
}
