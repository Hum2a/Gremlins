using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gremlins.Core;
using Gremlins.Services;
using Newtonsoft.Json;

namespace Gremlins.UI;

public partial class MainViewModel : ObservableObject
{
    private readonly GremlinEngine _engine;
    private readonly ThemeService _themeService;
    private readonly PreferencesService _preferencesService;
    private readonly ExecutionGate _gate;
    private readonly ActivityLogService _activityLog;

    public ObservableCollection<GremlinCardViewModel> GremlinCards { get; } = [];

    public IReadOnlyList<AppThemePreference> ThemeOptions { get; } = BuildThemeOrder();

    public Preferences Behavior => _preferencesService.Current;

    public ActivityLogService ActivityLog => _activityLog;

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

    public string AppVersionLabel => $"v{AppVersion.SemanticVersion}";

    [ObservableProperty]
    private bool _startWithWindows;

    [ObservableProperty]
    private AppThemePreference _themePreference = AppThemePreference.System;

    [ObservableProperty]
    private string _updateCheckMessage = "";

    [ObservableProperty]
    private bool _isPortableMode;

    /// <summary>Human-readable line for the App tab.</summary>
    public string PortableMarkerSummary =>
        IsPortableMode
            ? "Portable marker on disk — restart Gremlins to load settings from GremlinsData beside the exe."
            : "Standard mode — settings under %AppData%\\Gremlins.";

    public MainViewModel(
        GremlinEngine engine,
        ThemeService themeService,
        PreferencesService preferencesService,
        ExecutionGate gate,
        ActivityLogService activityLog)
    {
        _engine = engine;
        _themeService = themeService;
        _preferencesService = preferencesService;
        _gate = gate;
        _activityLog = activityLog;
    }

    public void Initialise()
    {
        IsPortableMode = PortablePaths.IsPortableModeRequested();
        StartWithWindows = RegistryStartupHelper.IsEnabled();
        ThemePreference = _themeService.Preference;

        foreach (var g in _engine.AllGremlins)
            GremlinCards.Add(new GremlinCardViewModel(g, this, _engine, _preferencesService));

        RefreshStatus();
        OnPropertyChanged(nameof(PortableMarkerSummary));
        _activityLog.Add("Gremlins dashboard opened.");

        Behavior.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(Preferences.StartupDelaySeconds) && StartWithWindows)
                RefreshStartupRegistration();
        };
    }

    partial void OnStartWithWindowsChanged(bool value) =>
        RegistryStartupHelper.SetEnabled(value, Math.Clamp(Behavior.StartupDelaySeconds, 0, 600));

    partial void OnIsPortableModeChanged(bool value) =>
        OnPropertyChanged(nameof(PortableMarkerSummary));

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
        foreach (var card in GremlinCards)
            card.IsEnabled = true;
        _engine.SaveState();
    }

    [RelayCommand]
    private void DisableAll()
    {
        foreach (var card in GremlinCards)
            card.IsEnabled = false;
        _engine.SaveState();
    }

    [RelayCommand]
    private void Panic() => _gate.TriggerPanic();

    [RelayCommand]
    private void ResumeTricks() => _gate.ClearPanic();

    [RelayCommand]
    private void ProfileQuiet()
    {
        foreach (var card in GremlinCards)
            card.IsEnabled = false;
        _engine.SaveState();
        _activityLog.Add("Profile: Quiet workplace (all gremlins off).");
    }

    [RelayCommand]
    private void ProfileMild()
    {
        foreach (var card in GremlinCards)
        {
            card.IsEnabled = true;
            card.Severity = Severity.Mischievous;
        }

        _engine.SaveState();
        _activityLog.Add("Profile: Mild chaos (all on, mischievous).");
    }

    [RelayCommand]
    private void ProfileChaos()
    {
        foreach (var card in GremlinCards)
        {
            card.IsEnabled = true;
            card.Severity = Severity.Unhinged;
        }

        _engine.SaveState();
        _activityLog.Add("Profile: Maximum chaos (all on, unhinged).");
    }

    [RelayCommand]
    private void ClearActivityLog() => _activityLog.Clear();

    [RelayCommand]
    private async Task CheckUpdatesAsync()
    {
        UpdateCheckMessage = "Checking…";
        var tag = await UpdateChecker.TryGetLatestReleaseTagAsync(Behavior.GitHubUpdateRepo.Trim())
            .ConfigureAwait(true);
        if (string.IsNullOrEmpty(tag))
        {
            UpdateCheckMessage = "No update info (set GitHub owner/repo or check network).";
            return;
        }

        var cur = AppVersion.SemanticVersion;
        var newer = UpdateChecker.IsNewerThanCurrent(tag, cur);
        UpdateCheckMessage = newer
            ? $"Newer tag on GitHub: {tag} (you have {cur}). Open Releases from the repo page."
            : $"Latest tag {tag}. You are up to date ({cur}).";
    }

    [RelayCommand]
    private void ExportRecipe()
    {
        var dlg = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "Gremlins recipe (*.json)|*.json",
            DefaultExt = ".json",
            FileName = "gremlins-recipe.json",
        };
        if (dlg.ShowDialog() != true)
            return;

        var dict = _engine.AllGremlins.ToDictionary(
            g => g.Id,
            g => new GremlinState { IsEnabled = g.IsEnabled, Severity = g.Severity });
        File.WriteAllText(dlg.FileName, JsonConvert.SerializeObject(dict, Formatting.Indented));
        _activityLog.Add($"Exported recipe to {dlg.FileName}.");
    }

    [RelayCommand]
    private void ImportRecipe()
    {
        var dlg = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "Gremlins recipe (*.json)|*.json|All files|*.*",
        };
        if (dlg.ShowDialog() != true)
            return;

        try
        {
            var json = File.ReadAllText(dlg.FileName);
            var dict = JsonConvert.DeserializeObject<Dictionary<string, GremlinState>>(json);
            if (dict is null || dict.Count == 0)
            {
                System.Windows.MessageBox.Show("That file did not contain gremlin settings.", "Gremlins",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _engine.ApplyGremlinStates(dict);
            foreach (var card in GremlinCards)
                card.PullFromEngine();
            RefreshStatus();
            _activityLog.Add($"Imported recipe from {dlg.FileName}.");
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Could not import: {ex.Message}", "Gremlins",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void EnablePortableMode()
    {
        PortablePaths.EnsureMarkerFileForPortableMode();
        IsPortableMode = true;
        System.Windows.MessageBox.Show(
            "Portable marker created next to Gremlins.exe.\r\nRestart the app to use the GremlinsData folder beside the executable.",
            "Portable mode",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    [RelayCommand]
    private void DisablePortableMode()
    {
        var r = System.Windows.MessageBox.Show(
            "Remove portable markers? After restart, settings will use %AppData%\\Gremlins again.\r\nCopy GremlinsData manually if you need to keep files.",
            "Portable mode",
            MessageBoxButton.OKCancel,
            MessageBoxImage.Warning);
        if (r != System.Windows.MessageBoxResult.OK)
            return;
        PortablePaths.RemovePortableMarker();
        IsPortableMode = false;
    }

    [RelayCommand]
    private void ResetOnboarding()
    {
        Behavior.OnboardingComplete = false;
        _preferencesService.SaveImmediate();
        System.Windows.MessageBox.Show(
            "Next time you open Gremlins, the welcome summary will appear again.",
            "Gremlins",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    [RelayCommand]
    private void OpenGithubReleases()
    {
        var repo = Behavior.GitHubUpdateRepo?.Trim();
        if (string.IsNullOrEmpty(repo) || !repo.Contains('/'))
        {
            System.Windows.MessageBox.Show("Enter a GitHub repository as owner/name first.", "Gremlins",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var url = $"https://github.com/{repo}/releases";
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url)
            {
                UseShellExecute = true,
            });
        }
        catch
        {
            /* ignore */
        }
    }

    /// <summary>Call after startup delay changes while startup-with-Windows is on.</summary>
    public void RefreshStartupRegistration()
    {
        if (StartWithWindows)
            RegistryStartupHelper.SetEnabled(true, Math.Clamp(Behavior.StartupDelaySeconds, 0, 600));
    }
}
