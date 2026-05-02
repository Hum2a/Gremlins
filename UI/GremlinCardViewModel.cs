using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gremlins.Core;
using Gremlins.Services;

namespace Gremlins.UI;

public partial class GremlinCardViewModel : ObservableObject
{
    private readonly IGremlin _gremlin;
    private readonly MainViewModel _parent;
    private readonly GremlinEngine _engine;
    private readonly PreferencesService _preferences;
    private bool _suppressPersist;

    public string Name        => _gremlin.Name;
    public string Description => _gremlin.Description;
    public string IconGlyph   => _gremlin.IconGlyph;

    [ObservableProperty]
    private bool _isEnabled;

    [ObservableProperty]
    private Severity _severity;

    public GremlinCardViewModel(IGremlin gremlin, MainViewModel parent, GremlinEngine engine,
        PreferencesService preferences)
    {
        _gremlin = gremlin;
        _parent  = parent;
        _engine  = engine;
        _preferences = preferences;
        _isEnabled = gremlin.IsEnabled;
        _severity  = gremlin.Severity;
    }

    public void PullFromEngine()
    {
        _suppressPersist = true;
        IsEnabled = _gremlin.IsEnabled;
        Severity = _gremlin.Severity;
        _suppressPersist = false;
    }

    partial void OnIsEnabledChanged(bool value)
    {
        _gremlin.IsEnabled = value;
        _parent.RefreshStatus();
        if (!_suppressPersist)
            _engine.SaveState();
        if (!_suppressPersist)
            UiSoundPlayback.PlayToggleSound(_preferences.Current);
    }

    partial void OnSeverityChanged(Severity value)
    {
        _gremlin.Severity = value;
        if (!_suppressPersist)
            _engine.SaveState();
    }

    public IEnumerable<Severity> Severities => Enum.GetValues<Severity>();

    [RelayCommand]
    private void OpenGremlinSettings()
    {
        var dlg = new GremlinSettingsWindow(_gremlin.Id, _preferences)
        {
            Owner = System.Windows.Application.Current.MainWindow,
        };
        dlg.ShowDialog();
    }
}
