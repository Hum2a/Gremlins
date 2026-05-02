using System.Windows;
using System.Windows.Media;
using WpfColor = System.Windows.Media.Color;
using WpfColorConverter = System.Windows.Media.ColorConverter;

namespace Gremlins.Services.Themes;

public static class ThemeResourceDictionaryFactory
{
    public static ResourceDictionary Create(ThemeColors c)
    {
        var d = new ResourceDictionary();
        void Brush(string key, string hex) => d[key] = CreateBrush(hex);

        Brush("BrushBgDeep", c.BgDeep);
        Brush("BrushBgCard", c.BgCard);
        Brush("BrushCardHover", c.CardHover);
        Brush("BrushAccentGreen", c.AccentGreen);
        Brush("BrushAccentGreenDim", c.AccentGreenDim);
        Brush("BrushAccentRed", c.AccentRed);
        Brush("BrushTextPrimary", c.TextPrimary);
        Brush("BrushTextMuted", c.TextMuted);
        Brush("BrushBorder", c.Border);
        var t = new SolidColorBrush(System.Windows.Media.Colors.Transparent);
        t.Freeze();
        d["BrushTransparent"] = t;

        Brush("BrushWindowBg", c.WindowBg);
        Brush("BrushBorderOuter", c.BorderOuter);
        Brush("BrushTitleBar", c.TitleBar);
        Brush("BrushTitleAccent", c.TitleAccent);
        Brush("BrushFooterBg", c.FooterBg);
        Brush("BrushCloseButtonFg", c.CloseButtonFg);

        Brush("BrushComboDropBg", c.ComboDropBg);
        Brush("BrushComboDropBorder", c.ComboDropBorder);
        Brush("BrushComboItemHover", c.ComboItemHover);
        Brush("BrushComboItemSelected", c.ComboItemSelected);
        Brush("BrushComboMultiHighlight", c.ComboMultiHighlight);
        Brush("BrushComboChromeBg", c.ComboChromeBg);
        Brush("BrushComboHoverBorder", c.ComboHoverBorder);
        d["ColorPrimaryGlow"] = (WpfColor)WpfColorConverter.ConvertFromString(c.ColorPrimaryGlow)!;

        Brush("BrushActionPrimaryBg", c.ActionPrimaryBg);
        Brush("BrushActionPrimaryBgHover", c.ActionPrimaryBgHover);
        Brush("BrushActionPrimaryBgPressed", c.ActionPrimaryBgPressed);
        Brush("BrushActionSecondaryBg", c.ActionSecondaryBg);
        Brush("BrushActionSecondaryHover", c.ActionSecondaryHover);
        Brush("BrushActionSecondaryBorderHover", c.ActionSecondaryBorderHover);
        Brush("BrushActionSecondaryPressed", c.ActionSecondaryPressed);

        Brush("BrushScrollBarTrack", "Transparent");
        return d;
    }

    private static SolidColorBrush CreateBrush(string hexOrName)
    {
        var brush = new SolidColorBrush((WpfColor)WpfColorConverter.ConvertFromString(hexOrName)!);
        brush.Freeze();
        return brush;
    }

    /// <summary>HSL in degrees 0–360, S/V in 0–1 → #RRGGBB.</summary>
    public static string HslToHex(double h, double s, double v)
    {
        h = ((h % 360) + 360) % 360;
        s = Math.Clamp(s, 0, 1);
        v = Math.Clamp(v, 0, 1);
        var c = v * s;
        var x = c * (1 - Math.Abs((h / 60 % 2) - 1));
        var m = v - c;
        double r1 = 0, g1 = 0, b1 = 0;
        if (h < 60) { r1 = c; g1 = x; }
        else if (h < 120) { r1 = x; g1 = c; }
        else if (h < 180) { g1 = c; b1 = x; }
        else if (h < 240) { g1 = x; b1 = c; }
        else if (h < 300) { r1 = x; b1 = c; }
        else { r1 = c; b1 = x; }
        byte R = ToByte((r1 + m) * 255);
        byte G = ToByte((g1 + m) * 255);
        byte B = ToByte((b1 + m) * 255);
        return $"#{R:X2}{G:X2}{B:X2}";
    }

    private static byte ToByte(double d) => (byte)Math.Clamp((int)Math.Round(d), 0, 255);

    /// <summary>Evenly stepped hues around the wheel (dark chrome).</summary>
    public static ThemeColors ChromaticPrismatic(int index, int total = 64)
    {
        if (total < 1) total = 64;
        var h = index * 360.0 / total;
        var deep = HslToHex(h, 0.14, 0.06);
        var card = HslToHex(h, 0.11, 0.10);
        var hover = HslToHex(h, 0.12, 0.13);
        var border = HslToHex(h, 0.08, 0.22);
        var accent = HslToHex(h, 0.82, 0.92);
        var accentDim = HslToHex(h, 0.55, 0.32);
        var red = HslToHex((h + 48) % 360, 0.75, 0.65);
        var text = HslToHex(h, 0.04, 0.92);
        var muted = HslToHex(h, 0.06, 0.48);
        var titleBar = HslToHex(h, 0.10, 0.08);
        var footer = titleBar;
        var dropBg = HslToHex(h, 0.12, 0.09);
        var dropBorder = HslToHex(h, 0.15, 0.28);
        var itemHover = HslToHex(h, 0.14, 0.16);
        var itemSel = HslToHex((h + 12) % 360, 0.35, 0.22);
        var multi = HslToHex((h + 8) % 360, 0.38, 0.26);
        var chrome = HslToHex(h, 0.10, 0.12);
        var hoverB = HslToHex((h + 25) % 360, 0.25, 0.42);
        var glow = $"#FF{accent.TrimStart('#')}";
        var apBg = HslToHex((h + 18) % 360, 0.28, 0.14);
        var apH = HslToHex((h + 18) % 360, 0.32, 0.20);
        var apP = HslToHex((h + 18) % 360, 0.30, 0.11);
        var sec = HslToHex(h, 0.08, 0.14);
        var secH = HslToHex(h, 0.09, 0.19);
        var secBH = HslToHex((h + 40) % 360, 0.22, 0.45);
        var secP = HslToHex(h, 0.08, 0.11);

        return new ThemeColors(
            BgDeep: deep,
            BgCard: card,
            CardHover: hover,
            AccentGreen: accent,
            AccentGreenDim: accentDim,
            AccentRed: red,
            TextPrimary: text,
            TextMuted: muted,
            Border: border,
            WindowBg: deep,
            BorderOuter: border,
            TitleBar: titleBar,
            TitleAccent: accent,
            FooterBg: footer,
            CloseButtonFg: muted,
            ComboDropBg: dropBg,
            ComboDropBorder: dropBorder,
            ComboItemHover: itemHover,
            ComboItemSelected: itemSel,
            ComboMultiHighlight: multi,
            ComboChromeBg: chrome,
            ComboHoverBorder: hoverB,
            ColorPrimaryGlow: glow,
            ActionPrimaryBg: apBg,
            ActionPrimaryBgHover: apH,
            ActionPrimaryBgPressed: apP,
            ActionSecondaryBg: sec,
            ActionSecondaryHover: secH,
            ActionSecondaryBorderHover: secBH,
            ActionSecondaryPressed: secP);
    }
}
