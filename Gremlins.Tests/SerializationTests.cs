using Gremlins.Core;
using Gremlins.Services;
using Newtonsoft.Json;
using Xunit;

namespace Gremlins.Tests;

public sealed class SerializationTests
{
    [Fact]
    public void Gremlin_state_dictionary_round_trips()
    {
        var original = new Dictionary<string, GremlinState>
        {
            ["the_typist"] = new GremlinState { IsEnabled = true, Severity = Severity.Unhinged },
            ["the_drifter"] = new GremlinState { IsEnabled = false, Severity = Severity.Mischievous },
        };

        var json = JsonConvert.SerializeObject(original);
        var copy = JsonConvert.DeserializeObject<Dictionary<string, GremlinState>>(json);

        Assert.NotNull(copy);
        Assert.Equal(original.Count, copy!.Count);
        Assert.True(copy["the_typist"].IsEnabled);
        Assert.Equal(Severity.Unhinged, copy["the_typist"].Severity);
    }

    [Fact]
    public void Preferences_round_trips_defaults()
    {
        var p = new Preferences
        {
            QuietHoursEnabled = true,
            FocusDenyExeList = "a.exe,b.exe",
            UiSoundPreset = UiSoundPreset.Exclamation,
            UiSoundCustomPath = @"C:\sfx\click.wav",
            UiSoundVolumePercent = 55,
        };
        var json = JsonConvert.SerializeObject(p);
        var q = JsonConvert.DeserializeObject<Preferences>(json);
        Assert.NotNull(q);
        Assert.True(q!.QuietHoursEnabled);
        Assert.Equal("a.exe,b.exe", q.FocusDenyExeList);
        Assert.Equal(UiSoundPreset.Exclamation, q.UiSoundPreset);
        Assert.Equal(@"C:\sfx\click.wav", q.UiSoundCustomPath);
        Assert.Equal(55, q.UiSoundVolumePercent);
    }

    [Fact]
    public void Preferences_round_trips_per_gremlin_sections()
    {
        var p = new Preferences();
        p.Drifter.UseCustomSettings = true;
        p.Drifter.MinIntervalSeconds = 99;
        p.Typist.SubstitutionChancePercent = 3.5;
        p.LagGhost.BurstDurationSeconds = 42;

        var json = JsonConvert.SerializeObject(p);
        var q = JsonConvert.DeserializeObject<Preferences>(json);

        Assert.NotNull(q);
        Assert.True(q!.Drifter.UseCustomSettings);
        Assert.Equal(99, q.Drifter.MinIntervalSeconds);
        Assert.Equal(3.5, q.Typist.SubstitutionChancePercent);
        Assert.Equal(42, q.LagGhost.BurstDurationSeconds);
    }
}
