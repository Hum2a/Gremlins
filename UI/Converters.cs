using System.Globalization;
using System.Windows.Data;
using Gremlins.Core;
using Gremlins.Services;
using Binding = System.Windows.Data.Binding;

namespace Gremlins.UI;

public class SeverityLabelConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Severity s)
            return string.Empty;
        return s switch
        {
            Severity.Mischievous => "😈 Mischievous",
            Severity.Annoying    => "😤 Annoying",
            Severity.Unhinged    => "🤯 Unhinged",
            _                    => s.ToString(),
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}

public class MinutesClockConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int m)
        {
            m = ((m % 1440) + 1440) % 1440;
            return $"{m / 60:D2}:{m % 60:D2}";
        }

        return string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}

public class ThemePreferenceLabelConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is AppThemePreference p
            ? ThemePreferenceLabels.GetDisplayName(p)
            : string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}

/// <summary>Live value readouts with explicit units for Gremlin Settings sliders.</summary>
public sealed class GremlinSettingUnitConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var kind = parameter as string ?? "";
        return kind switch
        {
            "Sec" => FormatSeconds(ToInt(value)),
            "Min" => FormatMinutes(ToInt(value)),
            "Ms" => FormatMillis(ToInt(value)),
            "Px" => $"{ToInt(value)} px per axis (horizontal / vertical)",
            "PctVol" => $"{ToInt(value)} % output volume",
            "PctKey" => $"{ToDouble(value):0.###} % chance per matching keystroke (lookalike map)",
            _ => value?.ToString() ?? "",
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();

    private static int ToInt(object? value) => value switch
    {
        int i => i,
        double d => (int)Math.Round(d),
        float f => (int)Math.Round(f),
        _ => 0,
    };

    private static double ToDouble(object? value) => value switch
    {
        double d => d,
        float f => f,
        int i => i,
        _ => 0,
    };

    private static string FormatSeconds(int s)
    {
        if (s <= 0) return "0 s";
        if (s < 60) return $"Currently {s} s";
        if (s < 3600) return $"Currently {s} s ({s / 60.0:0.#} min)";
        var h = s / 3600;
        var remMin = (s % 3600) / 60;
        return remMin == 0
            ? $"Currently {s} s ({h} h)"
            : $"Currently {s} s ({h} h {remMin} min)";
    }

    private static string FormatMinutes(int m)
    {
        if (m <= 0) return "0 min";
        if (m < 60) return $"Currently {m} min";
        var h = m / 60;
        var rem = m % 60;
        return rem == 0
            ? $"Currently {m} min ({h} h)"
            : $"Currently {m} min ({h} h {rem} min)";
    }

    private static string FormatMillis(int ms)
    {
        if (ms <= 0) return "0 ms";
        if (ms < 1000) return $"Currently {ms} ms (~{ms / 1000.0:0.###} s added delay per mouse message)";
        var s = ms / 1000.0;
        return $"Currently {ms} ms (~{s:0.#} s added delay per mouse message)";
    }
}

/// <summary>Two-way string ↔ number for gremlin numeric TextBoxes; clamps to min/max from parameter <c>I:min:max</c> or <c>D:min:max</c>.</summary>
public sealed class GremlinClampedNumericTextConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter is not string p || !TryParseParameter(p, out var isDouble, out var min, out var max))
            return value?.ToString() ?? "";

        if (isDouble)
        {
            var d = Math.Clamp(ToDouble(value), min, max);
            return d.ToString("0.###", culture);
        }

        var i = Math.Clamp(ToInt(value), (int)Math.Round(min), (int)Math.Round(max));
        return i.ToString(culture);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter is not string p || !TryParseParameter(p, out var isDouble, out var min, out var max))
            return Binding.DoNothing;

        var s = value as string;
        if (string.IsNullOrWhiteSpace(s))
            return Binding.DoNothing;

        if (isDouble)
        {
            if (!TryParseDouble(s, culture, out var d))
                return Binding.DoNothing;
            return Math.Clamp(d, min, max);
        }

        if (!TryParseInt(s, culture, out var i))
            return Binding.DoNothing;
        return Math.Clamp(i, (int)Math.Round(min), (int)Math.Round(max));
    }

    private static bool TryParseParameter(string p, out bool isDouble, out double min, out double max)
    {
        isDouble = false;
        min = max = 0;
        var parts = p.Split(':');
        if (parts.Length != 3)
            return false;
        if (string.Equals(parts[0], "D", StringComparison.OrdinalIgnoreCase))
            isDouble = true;
        else if (!string.Equals(parts[0], "I", StringComparison.OrdinalIgnoreCase))
            return false;

        return double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out min)
               && double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out max);
    }

    private static bool TryParseDouble(string s, CultureInfo culture, out double d)
    {
        if (double.TryParse(s.Trim(), NumberStyles.Float, culture, out d))
            return true;
        return double.TryParse(s.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out d);
    }

    private static bool TryParseInt(string s, CultureInfo culture, out int i)
    {
        if (int.TryParse(s.Trim(), NumberStyles.Integer, culture, out i))
            return true;
        return int.TryParse(s.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out i);
    }

    private static int ToInt(object? value) => value switch
    {
        int x => x,
        double d => (int)Math.Round(d),
        float f => (int)Math.Round(f),
        _ => 0,
    };

    private static double ToDouble(object? value) => value switch
    {
        double d => d,
        float f => f,
        int i => i,
        _ => 0,
    };
}
