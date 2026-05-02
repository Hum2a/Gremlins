using System.Text.RegularExpressions;
using Gremlins.Services.Themes;
using Xunit;

namespace Gremlins.Tests;

public sealed class ThemeMathTests
{
    private static readonly Regex Hex = new(@"^#[0-9A-Fa-f]{6}$", RegexOptions.Compiled);

    [Fact]
    public void HslToHex_produces_six_digit_hex()
    {
        Assert.Matches(Hex, ThemeResourceDictionaryFactory.HslToHex(120, 0.5, 0.5));
    }

    [Fact]
    public void Chromatic_prismatic_deterministic_per_index()
    {
        var a = ThemeResourceDictionaryFactory.ChromaticPrismatic(3, 64);
        var b = ThemeResourceDictionaryFactory.ChromaticPrismatic(3, 64);
        Assert.Equal(a.AccentGreen, b.AccentGreen);
        Assert.NotEqual(
            ThemeResourceDictionaryFactory.ChromaticPrismatic(3, 64).AccentGreen,
            ThemeResourceDictionaryFactory.ChromaticPrismatic(4, 64).AccentGreen);
    }
}
