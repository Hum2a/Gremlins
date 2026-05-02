using System.Globalization;
using System.Windows.Data;
using Gremlins.Core;

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
