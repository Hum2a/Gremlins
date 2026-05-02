using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gremlins.Core;
using Gremlins.Services;
using System.Collections.ObjectModel;

namespace Gremlins.UI;

public partial class MainViewModel : ObservableObject
{
    private readonly GremlinEngine _engine;
    private readonly ThemeService _themeService;

    public ObservableCollection<GremlinCardViewModel> GremlinCards { get; } = [];

    public IReadOnlyList<AppThemePreference> ThemeOptions { get; } = BuildThemeOrder();

    private static AppThemePreference[] BuildThemeOrder()
    {
        AppThemePreference[] head =
        [
            AppThemePreference.System,
            AppThemePreference.Dark,
            AppThemePreference.Light,
        ];
        var headSet = new HashSet<AppThemePreference>(head);
        var tail = Enum.GetValues<AppThemePreference>()
            .Where(p => !headSet.Contains(p))
            .OrderBy(ThemePreferenceLabels.GetDisplayName, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        return [..head, ..tail];
    }

    [ObservableProperty]
    private int _activeCount;

    [ObservableProperty]
    private string _statusText = "All quiet.";

    /// <summary>Shown in the window chrome (from assembly / csproj version).</summary>
    public string AppVersionLabel => $"v{AppVersion.SemanticVersion}";

    [ObservableProperty]
    private bool _startWithWindows;

    [ObservableProperty]
    private AppThemePreference _themePreference = AppThemePreference.System;

    public MainViewModel(GremlinEngine engine, ThemeService themeService)
    {
        _engine = engine;
        _themeService = themeService;
    }

    public void Initialise()
    {
        StartWithWindows = RegistryStartupHelper.IsEnabled();
        ThemePreference = _themeService.Preference;

        foreach (var g in _engine.AllGremlins)
            GremlinCards.Add(new GremlinCardViewModel(g, this, _engine));

        RefreshStatus();
    }

    partial void OnStartWithWindowsChanged(bool value) =>
        RegistryStartupHelper.SetEnabled(value);

    partial void OnThemePreferenceChanged(AppThemePreference value) =>
        _themeService.SetPreference(value);

    public void RefreshStatus()
    {
        ActiveCount = GremlinCards.Count(c => c.IsEnabled);
        StatusText = ActiveCount == 0
            ? "All quiet. Too quiet."
            : ActiveCount == 1
                ? "1 gremlin is loose."
                : $"{ActiveCount} gremlins are loose.";
    }

    [RelayCommand]
    private void EnableAll()
    {
        foreach (var card in GremlinCards) card.IsEnabled = true;
        _engine.SaveState();
    }

    [RelayCommand]
    private void DisableAll()
    {
        foreach (var card in GremlinCards) card.IsEnabled = false;
        _engine.SaveState();
    }
}
