using Gremlins.Services;

namespace Gremlins.UI;

/// <summary>Chooses which gremlin-specific panel is visible in <see cref="GremlinSettingsWindow"/>.</summary>
public sealed class GremlinSettingsViewModel
{
    public GremlinSettingsViewModel(string gremlinId, PreferencesService preferences)
    {
        GremlinId = gremlinId;
        Preferences = preferences;
    }

    public string GremlinId { get; }

    public PreferencesService Preferences { get; }

    public Preferences Current => Preferences.Current;

    public string WindowTitle => GremlinId switch
    {
        "the_drifter" => "The Drifter — settings",
        "the_typist" => "The Typist — settings",
        "the_amnesiac" => "The Amnesiac — settings",
        "the_critic" => "The Critic — settings",
        "the_philosopher" => "The Philosopher — settings",
        "the_lag_ghost" => "The Lag Ghost — settings",
        "the_rearranger" => "The Rearranger — settings",
        _ => "Gremlin settings",
    };

    public bool ShowDrifter => GremlinId == "the_drifter";

    public bool ShowTypist => GremlinId == "the_typist";

    public bool ShowAmnesiac => GremlinId == "the_amnesiac";

    public bool ShowCritic => GremlinId == "the_critic";

    public bool ShowPhilosopher => GremlinId == "the_philosopher";

    public bool ShowLagGhost => GremlinId == "the_lag_ghost";

    public bool ShowRearranger => GremlinId == "the_rearranger";
}
