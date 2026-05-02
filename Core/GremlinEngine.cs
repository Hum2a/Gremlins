using Gremlins.Tricks;
using Gremlins.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Gremlins.Core;

public class GremlinEngine
{
    private readonly IServiceProvider _services;
    private readonly SettingsService _settings;
    public IReadOnlyList<IGremlin> AllGremlins { get; private set; } = [];

    public GremlinEngine(IServiceProvider services, SettingsService settings)
    {
        _services = services;
        _settings = settings;
    }

    public void Initialise()
    {
        AllGremlins =
        [
            _services.GetRequiredService<TheDrifter>(),
            _services.GetRequiredService<TheTypist>(),
            _services.GetRequiredService<TheAmnesiac>(),
            _services.GetRequiredService<TheCritic>(),
            _services.GetRequiredService<ThePhilosopher>(),
            _services.GetRequiredService<TheLagGhost>(),
            _services.GetRequiredService<TheRearranger>(),
        ];

        // Restore saved state
        var saved = _settings.Load();
        foreach (var gremlin in AllGremlins)
        {
            if (saved.TryGetValue(gremlin.Id, out var state))
            {
                gremlin.Severity = state.Severity;
                gremlin.IsEnabled = state.IsEnabled; // triggers Start() if true
            }
        }
    }

    public void SaveState()
    {
        var dict = AllGremlins.ToDictionary(
            g => g.Id,
            g => new GremlinState { IsEnabled = g.IsEnabled, Severity = g.Severity }
        );
        _settings.Save(dict);
    }

    public void Shutdown()
    {
        SaveState();
        foreach (var g in AllGremlins)
            g.Stop();
    }
}
