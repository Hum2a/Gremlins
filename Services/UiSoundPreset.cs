namespace Gremlins.Services;

/// <summary>Built-in Windows notification sounds when no custom UI sound file is set.</summary>
/// <remarks><see cref="Asterisk"/> is 0 so older preference files without this field stay “classic”.</remarks>
public enum UiSoundPreset
{
    Asterisk = 0,
    None,
    Beep,
    Exclamation,
    Hand,
    Question,
}

public static class UiSoundPresetLabels
{
    public static string GetLabel(UiSoundPreset p) => p switch
    {
        UiSoundPreset.Asterisk => "Asterisk (classic)",
        UiSoundPreset.None => "None",
        UiSoundPreset.Beep => "Beep",
        UiSoundPreset.Exclamation => "Exclamation",
        UiSoundPreset.Hand => "Critical stop",
        UiSoundPreset.Question => "Question",
        _ => p.ToString(),
    };
}

/// <summary>For ComboBox: preset value + display label.</summary>
public sealed record UiSoundPresetOption(UiSoundPreset Preset, string Label);
