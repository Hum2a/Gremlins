using CommunityToolkit.Mvvm.ComponentModel;

namespace Gremlins.Services;

/// <summary>Optional per-gremlin tuning. When <see cref="UseCustomSettings"/> is false, severity presets apply.</summary>
public partial class DrifterGremlinSettings : ObservableObject
{
    [ObservableProperty] private bool _useCustomSettings;

    /// <summary>Minimum seconds between cursor nudges (custom mode).</summary>
    [ObservableProperty] private int _minIntervalSeconds = 240;

    /// <summary>Maximum seconds between cursor nudges (custom mode).</summary>
    [ObservableProperty] private int _maxIntervalSeconds = 480;

    /// <summary>Maximum drift in pixels each nudge (custom mode).</summary>
    [ObservableProperty] private int _maxDriftPixels = 4;
}

public partial class TypistGremlinSettings : ObservableObject
{
    [ObservableProperty] private bool _useCustomSettings;

    /// <summary>Chance each eligible keystroke is swapped (0.05–25%).</summary>
    [ObservableProperty] private double _substitutionChancePercent = 0.8;
}

public partial class AmnesiacGremlinSettings : ObservableObject
{
    [ObservableProperty] private bool _useCustomSettings;

    [ObservableProperty] private int _minIntervalMinutes = 10;

    [ObservableProperty] private int _maxIntervalMinutes = 20;
}

public partial class CriticGremlinSettings : ObservableObject
{
    [ObservableProperty] private bool _useCustomSettings;

    [ObservableProperty] private int _pollIntervalSeconds = 2;

    /// <summary>Extra comma-separated substrings to match in the foreground window title (case-insensitive).</summary>
    [ObservableProperty] private string _extraTitleKeywords = "";

    /// <summary>Loudness for built-in generated sigh or <see cref="CustomSighSoundPath"/> (1–100).</summary>
    [ObservableProperty] private int _sighVolumePercent = 30;

    /// <summary>Optional .wav / .mp3; overrides generated sigh when file exists.</summary>
    [ObservableProperty] private string _customSighSoundPath = "";
}

public partial class PhilosopherGremlinSettings : ObservableObject
{
    [ObservableProperty] private bool _useCustomSettings;

    [ObservableProperty] private int _minIntervalMinutes = 15;

    [ObservableProperty] private int _maxIntervalMinutes = 30;
}

public partial class LagGhostGremlinSettings : ObservableObject
{
    [ObservableProperty] private bool _useCustomSettings;

    [ObservableProperty] private int _outerMinSeconds = 20 * 60;

    [ObservableProperty] private int _outerMaxSeconds = 40 * 60;

    [ObservableProperty] private int _burstDurationSeconds = 20;

    [ObservableProperty] private int _lagDelayMinMs = 150;

    [ObservableProperty] private int _lagDelayMaxMs = 300;
}

public partial class RearrangerGremlinSettings : ObservableObject
{
    [ObservableProperty] private bool _useCustomSettings;

    [ObservableProperty] private int _minIntervalMinutes = 3 * 60;

    [ObservableProperty] private int _maxIntervalMinutes = 6 * 60;

    [ObservableProperty] private int _maxNudgePixels = 8;
}
