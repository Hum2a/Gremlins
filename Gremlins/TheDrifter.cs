using Gremlins.Core;

namespace Gremlins.Gremlins;

/// <summary>
/// Subtly drifts the mouse cursor a few pixels every so often.
/// Mischievous: 4–8 min interval, 2–4px drift
/// Annoying:    2–4 min interval, 4–8px drift
/// Unhinged:    30–60s interval,  8–20px drift
/// </summary>
public class TheDrifter : BaseGremlin
{
    public override string Id          => "the_drifter";
    public override string Name        => "The Drifter";
    public override string Description => "Nudges your cursor a few pixels when you're not looking. Completely deniable.";
    public override string Emoji       => "🖱️";

    protected override async Task RunLoopAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var intervalMs = Severity switch
            {
                Severity.Mischievous => RandomBetween(4 * 60_000, 8 * 60_000),
                Severity.Annoying    => RandomBetween(2 * 60_000, 4 * 60_000),
                Severity.Unhinged    => RandomBetween(30_000, 60_000),
                _                    => 5 * 60_000
            };

            await Task.Delay(intervalMs, ct);
            if (ct.IsCancellationRequested) break;

            DriftCursor();
        }
    }

    private void DriftCursor()
    {
        if (!Win32.GetCursorPos(out var pos)) return;

        int maxDrift = Severity switch
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
