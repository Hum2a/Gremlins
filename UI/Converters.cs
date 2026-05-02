using System.Globalization;
using System.Windows.Data;
using Gremlins.Core;
using Gremlins.Services;

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
