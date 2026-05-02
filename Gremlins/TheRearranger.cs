using Gremlins.Core;

namespace Gremlins.Gremlins;

/// <summary>
/// Every few hours, nudges the position of the currently focused window
/// by a small amount. You'll never be able to prove anything.
/// </summary>
public class TheRearranger : BaseGremlin
{
    public override string Id          => "the_rearranger";
    public override string Name        => "The Rearranger";
    public override string Description => "Slowly shifts your active window's position over time. Nothing looks right but you can't explain why.";
    public override string Emoji       => "🪄";

    protected override async Task RunLoopAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var intervalMs = Severity switch
            {
                Severity.Mischievous => RandomBetween(3 * 60 * 60_000, 6 * 60 * 60_000), // 3–6 hrs
                Severity.Annoying    => RandomBetween(60 * 60_000, 2 * 60 * 60_000),      // 1–2 hrs
                Severity.Unhinged    => RandomBetween(15 * 60_000, 30 * 60_000),          // 15–30 min
                _                    => 4 * 60 * 60_000
            };

            await Task.Delay(intervalMs, ct);
            if (ct.IsCancellationRequested) break;

            NudgeActiveWindow();
        }
    }

    private void NudgeActiveWindow()
    {
        var hwnd = Win32.GetForegroundWindow();
        if (hwnd == IntPtr.Zero) return;
        if (!Win32.GetWindowRect(hwnd, out var rect)) return;

        int maxNudge = Severity switch
        {
            Severity.Mischievous => 8,
            Severity.Annoying    => 20,
            Severity.Unhinged    => 50,
            _                    => 8
        };

        int dx = RandomBetween(-maxNudge, maxNudge);
        int dy = RandomBetween(-maxNudge, maxNudge);

        int screenW = Win32.GetSystemMetrics(Win32.SM_CXSCREEN);
        int screenH = Win32.GetSystemMetrics(Win32.SM_CYSCREEN);

        int newX = Math.Clamp(rect.Left + dx, 0, screenW - 200);
        int newY = Math.Clamp(rect.Top  + dy, 0, screenH - 100);

        Win32.SetWindowPos(
            hwnd,
            IntPtr.Zero,
            newX, newY,
            0, 0,
            Win32.SWP_NOSIZE | Win32.SWP_NOZORDER | Win32.SWP_NOACTIVATE
        );
    }
}
