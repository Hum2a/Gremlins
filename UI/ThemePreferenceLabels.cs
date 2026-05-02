using Gremlins.Services;

namespace Gremlins.UI;

public static class ThemePreferenceLabels
{
    public static string GetDisplayName(AppThemePreference p) => p switch
    {
        AppThemePreference.System => "Follow system",
        AppThemePreference.Dark => "Dark (classic gremlin)",
        AppThemePreference.Light => "Light (classic gremlin)",
        AppThemePreference.Dracula => "Dracula",
        AppThemePreference.Nord => "Nord",
        AppThemePreference.GruvboxDark => "Gruvbox dark",
        AppThemePreference.GruvboxLight => "Gruvbox light",
        AppThemePreference.SolarizedDark => "Solarized dark",
        AppThemePreference.SolarizedLight => "Solarized light",
        AppThemePreference.TokyoNight => "Tokyo Night",
        AppThemePreference.CatppuccinMocha => "Catppuccin mocha",
        AppThemePreference.CatppuccinLatte => "Catppuccin latte",
        AppThemePreference.OneDark => "One Dark",
        AppThemePreference.Monokai => "Monokai",
        AppThemePreference.RosePine => "Rosé pine",
        AppThemePreference.Everforest => "Everforest",
        AppThemePreference.AyuMirage => "Ayu mirage",
        AppThemePreference.NightOwl => "Night owl",
        AppThemePreference.ShadesOfPurple => "Shades of purple",
        AppThemePreference.Synthwave84 => "Synthwave '84",
        AppThemePreference.MatrixGreen => "Matrix green",
        AppThemePreference.AmberCRT => "Amber CRT",
        AppThemePreference.HotDogStand => "Hot dog stand (cursed)",
        AppThemePreference.VaporwavePastel => "Vaporwave pastel",
        AppThemePreference.PaperWhite => "Editorial newsprint",
        AppThemePreference.GlitchCore => "Glitchcore",
        AppThemePreference.UnicornAcid => "Unicorn acid",
        AppThemePreference.BeigeNightmare => "Beige nightmare",
        AppThemePreference.MiamiVice => "Miami vice",
        AppThemePreference.CorpBland => "Corporate beige",
        AppThemePreference.ZombieMold => "Zombie mold",
        AppThemePreference.Steampunk => "Steampunk brass",
        AppThemePreference.ClippyAssist => "Clippy & Paperclip blue",
        _ => PrismaticLabel(p) ?? p.ToString(),
    };

    private static string? PrismaticLabel(AppThemePreference p)
    {
        var s = p.ToString();
        if (s.StartsWith("Prismatic", StringComparison.Ordinal) && s.Length >= 11 &&
            int.TryParse(s.AsSpan(9), out var n) && n is >= 1 and <= 64)
            return $"Prismatic {n:00} (full hue ring · 1/64)";
        return null;
    }
}
