using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gremlins.Core;
using System.Collections.ObjectModel;

namespace Gremlins.UI;

public partial class MainViewModel : ObservableObject
{
    private readonly GremlinEngine _engine;

    public ObservableCollection<GremlinCardViewModel> GremlinCards { get; } = [];

    [ObservableProperty]
    private int _activeCount;

    [ObservableProperty]
    private string _statusText = "All quiet.";

    public MainViewModel(GremlinEngine engine)
    {
        _engine = engine;
    }

    public void Initialise()
    {
        foreach (var g in _engine.AllGremlins)
        {
            var card = new GremlinCardViewModel(g, this);
            GremlinCards.Add(card);
        }
        RefreshStatus();
    }

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
    }

    [RelayCommand]
    private void DisableAll()
    {
        foreach (var card in GremlinCards) card.IsEnabled = false;
    }
}

public partial class GremlinCardViewModel : ObservableObject
{
    private readonly IGremlin _gremlin;
    private readonly MainViewModel _parent;

    public string Name        => _gremlin.Name;
    public string Description => _gremlin.Description;
    public string Emoji       => _gremlin.Emoji;

    [ObservableProperty]
    private bool _isEnabled;

    [ObservableProperty]
    private Severity _severity;

    public GremlinCardViewModel(IGremlin gremlin, MainViewModel parent)
    {
        _gremlin = gremlin;
        _parent  = parent;
        _isEnabled = gremlin.IsEnabled;
        _severity  = gremlin.Severity;
    }

    partial void OnIsEnabledChanged(bool value)
    {
        _gremlin.IsEnabled = value;
        _parent.RefreshStatus();
    }

    partial void OnSeverityChanged(Severity value)
    {
        _gremlin.Severity = value;
    }

    public IEnumerable<Severity> Severities =>
        Enum.GetValues<Severity>();
}
