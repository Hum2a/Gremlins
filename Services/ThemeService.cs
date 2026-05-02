using System.Windows;
using Gremlins.Services.Themes;
using Microsoft.Win32;

namespace Gremlins.Services;

/// <summary>
/// Swaps the theme <see cref="ResourceDictionary"/> and keeps <see cref="AppThemePreference"/> in sync with disk.
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
        ThemePalettes.IsDarkAppearance(Preference, IsWindowsLightMode());

    public void SetPreference(AppThemePreference preference)
    {
        Preference = preference;
        _uiSettings.Save(new UiSettings { ThemePreference = preference });
        ApplyResolvedTheme();
    }

    private void ApplyResolvedTheme()
    {
        ThemeColors colors;
        if (Preference == AppThemePreference.System)
            colors = IsWindowsLightMode() ? ThemePalettes.Light : ThemePalettes.Dark;
        else if (ThemePalettes.TryGet(Preference, out var entry))
            colors = entry.Colors;
        else
            colors = ThemePalettes.Dark;

        MergeThemeDictionary(ThemeResourceDictionaryFactory.Create(colors));
    }

    /// <summary>Theme dictionary must stay merged at index 0 (before <c>Styles.xaml</c>).</summary>
    private void MergeThemeDictionary(ResourceDictionary next)
    {
        var app = System.Windows.Application.Current;
        var merged = app.Resources.MergedDictionaries;

        if (_themeDictionary is not null)
            merged.Remove(_themeDictionary);
        else if (merged.Count > 0)
            merged.RemoveAt(0);

        merged.Insert(0, next);
        _themeDictionary = next;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged;
    }
}
