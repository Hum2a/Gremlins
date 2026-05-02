using System.Linq;
using System.Windows;
using Microsoft.Win32;

namespace Gremlins.Services;

/// <summary>
/// Swaps theme resource dictionaries and tracks Dark / Light / Follow system (Windows Apps theme).
/// </summary>
public sealed class ThemeService : IDisposable
{
    private readonly UiSettingsService _uiSettings;
    private ResourceDictionary? _themeDictionary;
    private bool _disposed;

    public AppThemePreference Preference { get; private set; } = AppThemePreference.System;

    public ThemeService(UiSettingsService uiSettings)
    {
        _uiSettings = uiSettings;
    }

    public void Initialize()
    {
        var loaded = _uiSettings.Load();
        Preference = loaded.ThemePreference;
        ApplyResolvedTheme();

        SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
    }

    private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
    {
        if (Preference != AppThemePreference.System)
            return;
        // Dark/light toggle in Windows Settings → Personalization
        if (e.Category != UserPreferenceCategory.General && e.Category != UserPreferenceCategory.Color)
            return;
        System.Windows.Application.Current.Dispatcher.Invoke(ApplyResolvedTheme);
    }

    /// <summary>
    /// HKCU ...\Personalize\AppsUseLightTheme — 1 = light apps, 0 = dark apps (Windows 10/11).
    /// </summary>
    public static bool IsWindowsLightMode()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", false);
            var v = key?.GetValue("AppsUseLightTheme");
            return v is int i ? i != 0 : true;
        }
        catch
        {
            return true;
        }
    }

    public bool IsEffectiveDarkTheme =>
        Preference switch
        {
            AppThemePreference.Dark   => true,
            AppThemePreference.Light  => false,
            AppThemePreference.System => !IsWindowsLightMode(),
            _                         => true,
        };

    public void SetPreference(AppThemePreference preference)
    {
        Preference = preference;
        _uiSettings.Save(new UiSettings { ThemePreference = preference });
        ApplyResolvedTheme();
    }

    private void ApplyResolvedTheme() =>
        MergeTheme(IsEffectiveDarkTheme);

    private void MergeTheme(bool dark)
    {
        var app = System.Windows.Application.Current;
        var merged = app.Resources.MergedDictionaries;

        var uri = new Uri(
            dark
                ? "pack://application:,,,/UI/Themes/Theme.Dark.xaml"
                : "pack://application:,,,/UI/Themes/Theme.Light.xaml",
            UriKind.Absolute);

        var next = new ResourceDictionary { Source = uri };

        if (_themeDictionary is not null)
            merged.Remove(_themeDictionary);
        else if (merged.Any() && IsThemeDictionary(merged[0]))
            merged.RemoveAt(0);

        merged.Insert(0, next);
        _themeDictionary = next;
    }

    private static bool IsThemeDictionary(ResourceDictionary d)
    {
        var s = d.Source?.ToString() ?? "";
        return s.Contains("Theme.Dark", StringComparison.OrdinalIgnoreCase)
            || s.Contains("Theme.Light", StringComparison.OrdinalIgnoreCase);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged;
    }
}
