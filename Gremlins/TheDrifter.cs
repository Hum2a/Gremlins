using Gremlins.Core;
using Gremlins.Services;

namespace Gremlins.Tricks;

/// <summary>
/// Subtly drifts the mouse cursor a few pixels every so often.
/// Mischievous: 4–8 min interval, 2–4px drift
/// Annoying:    2–4 min interval, 4–8px drift
/// Unhinged:    30–60s interval,  8–20px drift
/// </summary>
public class TheDrifter : BaseGremlin
{
    public TheDrifter(ExecutionGate gate, PreferencesService prefs) : base(gate, prefs) { }

    public override string Id          => "the_drifter";
    public override string Name        => "The Drifter";
    public override string Description => "Nudges your cursor a few pixels when you're not looking. Completely deniable.";
    public override string Emoji       => "🖱️";

    protected override async Task RunLoopAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var intervalMs = NextIntervalMs();
            intervalMs = ApplyIdleBoost(intervalMs);
            await Task.Delay(intervalMs, ct);
            if (ct.IsCancellationRequested) break;

            if (!Gate.ShouldExecute())
                continue;

            DriftCursor();
            Gate.LogGremlin(Name, "nudged the cursor");
        }
    }

    private int NextIntervalMs()
    {
        var d = Prefs.Current.Drifter;
        if (d.UseCustomSettings)
        {
            var lo = Math.Clamp(Math.Min(d.MinIntervalSeconds, d.MaxIntervalSeconds), 10, 7200);
            var hi = Math.Clamp(Math.Max(d.MinIntervalSeconds, d.MaxIntervalSeconds), lo, 7200);
            return RandomBetween(lo * 1000, hi * 1000);
        }

        return Severity switch
        {
            Severity.Mischievous => RandomBetween(4 * 60_000, 8 * 60_000),
            Severity.Annoying    => RandomBetween(2 * 60_000, 4 * 60_000),
            Severity.Unhinged    => RandomBetween(30_000, 60_000),
            _                    => 5 * 60_000
        };
    }

    private void DriftCursor()
    {
        if (!Win32.GetCursorPos(out var pos)) return;

        int maxDrift = Prefs.Current.Drifter.UseCustomSettings
            ? Math.Clamp(Prefs.Current.Drifter.MaxDriftPixels, 1, 120)
            : Severity switch
            {
                Severity.Mischievous => 4,
                Severity.Annoying    => 8,
                Severity.Unhinged    => 20,
                _                    => 4
            };

        int dx = RandomBetween(-maxDrift, maxDrift);
        int dy = RandomBetween(-maxDrift, maxDrift);

        int screenW = Win32.GetSystemMetrics(Win32.SM_CXSCREEN);
        int screenH = Win32.GetSystemMetrics(Win32.SM_CYSCREEN);

        int newX = Math.Clamp(pos.X + dx, 0, screenW);
        int newY = Math.Clamp(pos.Y + dy, 0, screenH);

        Win32.SetCursorPos(newX, newY);
    }
}
