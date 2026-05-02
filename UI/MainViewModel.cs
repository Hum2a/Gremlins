using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gremlins.Core;
using Gremlins.Services;
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

    [ObservableProperty]
    private bool _startWithWindows;

    public MainViewModel(GremlinEngine engine)
    {
        _engine = engine;
    }

    public void Initialise()
    {
        StartWithWindows = RegistryStartupHelper.IsEnabled();

        foreach (var g in _engine.AllGremlins)
            GremlinCards.Add(new GremlinCardViewModel(g, this, _engine));

        RefreshStatus();
    }

    partial void OnStartWithWindowsChanged(bool value) =>
        RegistryStartupHelper.SetEnabled(value);

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
