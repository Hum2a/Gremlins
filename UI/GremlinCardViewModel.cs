using CommunityToolkit.Mvvm.ComponentModel;
using Gremlins.Core;

namespace Gremlins.UI;

public partial class GremlinCardViewModel : ObservableObject
{
    private readonly IGremlin _gremlin;
    private readonly MainViewModel _parent;
    private readonly GremlinEngine _engine;

    public string Name        => _gremlin.Name;
    public string Description => _gremlin.Description;
    public string Emoji       => _gremlin.Emoji;

    [ObservableProperty]
    private bool _isEnabled;

    [ObservableProperty]
    private Severity _severity;

    public GremlinCardViewModel(IGremlin gremlin, MainViewModel parent, GremlinEngine engine)
    {
        _gremlin = gremlin;
        _parent  = parent;
        _engine  = engine;
        _isEnabled = gremlin.IsEnabled;
        _severity  = gremlin.Severity;
    }

    partial void OnIsEnabledChanged(bool value)
    {
        _gremlin.IsEnabled = value;
        _parent.RefreshStatus();
        _engine.SaveState();
    }

    partial void OnSeverityChanged(Severity value)
    {
        _gremlin.Severity = value;
        _engine.SaveState();
    }

    public IEnumerable<Severity> Severities => Enum.GetValues<Severity>();
}
