using CommunityToolkit.Mvvm.ComponentModel;

namespace Gremlins.Services;

/// <summary>User-editable behavior, safety, and convenience settings (JSON persisted).</summary>
public partial class Preferences : ObservableObject
{
    [ObservableProperty] private bool _onboardingComplete;

    [ObservableProperty] private bool _activityLogEnabled = true;

    [ObservableProperty] private bool _uiSoundsEnabled;

    [ObservableProperty] private bool _quietHoursEnabled;

    /// <summary>Minutes from midnight when quiet hours start (e.g. 1320 = 22:00).</summary>
    [ObservableProperty] private int _quietStartMinutes = 22 * 60;

    /// <summary>Minutes from midnight when quiet hours end (e.g. 420 = 07:00).</summary>
    [ObservableProperty] private int _quietEndMinutes = 7 * 60;

    [ObservableProperty] private bool _scheduleEnabled;

    /// <summary>If true, gremlins only run when local hour is in [<see cref="ScheduleFromHour"/>, <see cref="ScheduleToHour"/>).</summary>
    [ObservableProperty] private int _scheduleFromHour = 17;

    [ObservableProperty] private int _scheduleToHour = 23;

    [ObservableProperty] private bool _scheduleWeekdaysOnly;

    [ObservableProperty] private bool _scheduleWeekendsOnly;

    [ObservableProperty] private bool _focusRulesEnabled;

    /// <summary>Comma-separated process names (e.g. <c>notepad.exe,devenv.exe</c>) — block tricks while one is in foreground.</summary>
    [ObservableProperty] private string _focusDenyExeList = "";

    [ObservableProperty] private bool _respectPresentationMode = true;

    /// <summary>Suppress tricks during exclusive full-screen / busy notification states.</summary>
    [ObservableProperty] private bool _respectFullScreenApps = true;

    [ObservableProperty] private bool _idleIntensityEnabled;

    /// <summary>After this many seconds of no input, intervals tighten (more chaos).</summary>
    [ObservableProperty] private int _idleBoostAfterSeconds = 300;

    /// <summary>After launch, block all tricks for this many minutes (in-app soft start).</summary>
    [ObservableProperty] private int _softStartMinutes;

    /// <summary>When starting with Windows, wait this many seconds before launching (registry Run).</summary>
    [ObservableProperty] private int _startupDelaySeconds;

    /// <summary>Optional <c>owner/repo</c> for GitHub release checks (e.g. <c>octocat/Hello-World</c>).</summary>
    [ObservableProperty] private string _gitHubUpdateRepo = "";

    /// <summary>Default duration for tray “panic” cooldown.</summary>
    [ObservableProperty] private int _panicCooldownMinutes = 30;
}
