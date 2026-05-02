using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace Gremlins;

/// <summary>
/// Builds a multi-resolution gremlin face icon (16 / 32 / 48) for tray + taskbar,
/// drawn with gradients so it reads clearly at small sizes.
/// </summary>
public static class IconGenerator
{
    private static System.Drawing.Icon? _cachedIcon;
    private static System.Windows.Media.ImageSource? _cachedWindowIcon;

    public static System.Drawing.Icon CreateGremlinIcon()
    {
        if (_cachedIcon is not null)
            return _cachedIcon;

        var png16 = BitmapToPngBytes(RenderGremlin(16));
        var png32 = BitmapToPngBytes(RenderGremlin(32));
        var png48 = BitmapToPngBytes(RenderGremlin(48));

        var icoBytes = BuildPngIco(png16, 16, 16, png32, 32, 32, png48, 48, 48);
        using var ms = new MemoryStream(icoBytes, writable: false);
        _cachedIcon = new System.Drawing.Icon(ms, 32, 32);
        return _cachedIcon;
    }

    /// <summary>WPF window / taskbar button icon (same artwork as tray).</summary>
    public static System.Windows.Media.ImageSource CreateWindowIconSource()
    {
        if (_cachedWindowIcon is not null)
            return _cachedWindowIcon;

        var icon = CreateGremlinIcon();
        using var bmp = icon.ToBitmap();
        var hBitmap = bmp.GetHbitmap(System.Drawing.Color.FromArgb(0, 0, 0, 0));
        try
        {
            _cachedWindowIcon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                System.Windows.Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            _cachedWindowIcon.Freeze();
        }
        finally
        {
            _ = DeleteObject(hBitmap);
        }

        return _cachedWindowIcon;
    }

    private static Bitmap RenderGremlin(int size)
    {
        var bmp = new Bitmap(size, size, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        using var g = Graphics.FromImage(bmp);
        float s = size / 32f;

        g.Clear(System.Drawing.Color.Transparent);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.PixelOffsetMode = PixelOffsetMode.Half;
        g.CompositingQuality = CompositingQuality.HighQuality;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;

        float penW = Math.Max(0.8f, 1.1f * s);

        // Soft cast shadow
        using (var shadow = new SolidBrush(System.Drawing.Color.FromArgb(55, 0, 0, 0)))
            g.FillEllipse(shadow, 5.5f * s, 23f * s, 21f * s, 7f * s);

        var head = new RectangleF(3.2f * s, 5f * s, 25.6f * s, 23.5f * s);

        // Head volume — vertical gradient
        using (var brush = new LinearGradientBrush(
                   head,
                   System.Drawing.Color.FromArgb(46, 168, 62),
                   System.Drawing.Color.FromArgb(10, 52, 20),
                   LinearGradientMode.Vertical))
        {
            g.FillEllipse(brush, head);
        }

        // Cheek / forehead highlight (gloss)
        using (var gloss = new SolidBrush(System.Drawing.Color.FromArgb(70, 200, 255, 200)))
            g.FillEllipse(gloss, 7f * s, 7.5f * s, 18f * s, 13f * s);

        // Inner rim light (neon-adjacent)
        using (var rim = new Pen(System.Drawing.Color.FromArgb(110, 57, 255, 40), penW * 0.65f))
        {
            rim.Alignment = PenAlignment.Inset;
            g.DrawEllipse(rim, head);
        }

        // Outer silhouette — reads on light + dark taskbars
        using (var outline = new Pen(System.Drawing.Color.FromArgb(220, 6, 28, 10), penW))
        {
            outline.LineJoin = LineJoin.Round;
            g.DrawEllipse(outline, head);
        }

        // Horns
        using var hornBrush = new LinearGradientBrush(
            new RectangleF(0, 0, size, size),
            System.Drawing.Color.FromArgb(55, 140, 48),
            System.Drawing.Color.FromArgb(15, 48, 18),
            55f);
        var leftHorn = new[]
        {
            new PointF(4f * s, 11f * s),
            new PointF(0.5f * s, 2f * s),
            new PointF(9f * s, 7f * s),
        };
        var rightHorn = new[]
        {
            new PointF(28f * s, 11f * s),
            new PointF(31.5f * s, 2f * s),
            new PointF(23f * s, 7f * s),
        };
        g.FillPolygon(hornBrush, leftHorn);
        g.FillPolygon(hornBrush, rightHorn);
        using (var hornEdge = new Pen(System.Drawing.Color.FromArgb(160, 10, 42, 14), penW * 0.55f))
        {
            g.DrawPolygon(hornEdge, leftHorn);
            g.DrawPolygon(hornEdge, rightHorn);
        }

        // Eyes — sclera + iris + pupil + catchlight
        DrawEye(g, s, 9f, 14f);
        DrawEye(g, s, 19f, 14f);

        // Mischievous grin — neon arc
        using (var grin = new Pen(System.Drawing.Color.FromArgb(250, 57, 255, 40), Math.Max(1f, 1.35f * s)))
        {
            grin.StartCap = LineCap.Round;
            grin.EndCap = LineCap.Round;
            g.DrawArc(grin, 9.5f * s, 15f * s, 13f * s, 12f * s, 15f, 150f);
        }

        // Subtle darker smile line
        using (var grinInner = new Pen(System.Drawing.Color.FromArgb(90, 0, 0, 0), penW * 0.45f))
            g.DrawArc(grinInner, 10f * s, 15.5f * s, 12f * s, 11f * s, 20f, 140f);

        return bmp;
    }

    private static void DrawEye(Graphics g, float s, float cx, float cy)
    {
        float rOuter = 4.2f * s;
        float rIris = 3f * s;
        float rPupil = 1.55f * s;

        var irisRect = new RectangleF((cx - rIris) * s, (cy - rIris) * s, 2 * rIris * s, 2 * rIris * s);

        // Outer glow ring
        using (var glow = new Pen(System.Drawing.Color.FromArgb(100, 57, 255, 40), Math.Max(0.6f, s)))
            g.DrawEllipse(glow, (cx - rOuter) * s, (cy - rOuter) * s, 2 * rOuter * s, 2 * rOuter * s);

        // Sclera
        using (var sclera = new SolidBrush(System.Drawing.Color.FromArgb(245, 248, 242)))
            g.FillEllipse(sclera, irisRect);

        // Iris
        using (var iris = new LinearGradientBrush(
                   irisRect,
                   System.Drawing.Color.FromArgb(255, 255, 230, 60),
                   System.Drawing.Color.FromArgb(255, 200, 140, 0),
                   LinearGradientMode.Vertical))
            g.FillEllipse(iris, irisRect);

        // Pupil
        using (var pupil = new SolidBrush(System.Drawing.Color.FromArgb(250, 8, 8, 8)))
            g.FillEllipse(pupil, (cx - rPupil) * s, (cy - rPupil + 0.2f) * s, 2 * rPupil * s, 2 * rPupil * s);

        // Specular
        float sp = 0.85f * s;
        using (var spec = new SolidBrush(System.Drawing.Color.FromArgb(240, 255, 255, 255)))
            g.FillEllipse(spec, (cx - 1.1f) * s, (cy - 1.8f) * s, sp * 2, sp * 2);
    }

    private static byte[] BitmapToPngBytes(Bitmap bmp)
    {
        using var ms = new MemoryStream();
        bmp.Save(ms, ImageFormat.Png);
        return ms.ToArray();
    }

    /// <summary>Windows Vista+ ICO containing embedded PNG images.</summary>
    private static byte[] BuildPngIco(
        byte[] png16, int w16, int h16,
        byte[] png32, int w32, int h32,
        byte[] png48, int w48, int h48)
    {
        var images = new[] { (png16, w16, h16), (png32, w32, h32), (png48, w48, h48) };
        int count = images.Length;
        int headerSize = 6 + 16 * count;
        var offsets = new int[count];
        int cursor = headerSize;
        for (int i = 0; i < count; i++)
        {
            offsets[i] = cursor;
            cursor += images[i].Item1.Length;
        }

        using var ms = new MemoryStream();
        using (var w = new BinaryWriter(ms))
        {
            w.Write((short)0);
            w.Write((short)1);
            w.Write((short)count);

            for (int i = 0; i < count; i++)
            {
                var (png, width, height) = images[i];
                w.Write((byte)(width >= 256 ? 0 : width));
                w.Write((byte)(height >= 256 ? 0 : height));
                w.Write((byte)0);
                w.Write((byte)0);
                w.Write((short)1);
                w.Write((short)32);
                w.Write(png.Length);
                w.Write(offsets[i]);
            }

            foreach (var tuple in images)
                w.Write(tuple.Item1);
        }

        return ms.ToArray();
    }

    [DllImport("gdi32.dll", SetLastError = true)]
    private static extern bool DeleteObject(IntPtr ho);
}
