using Gremlins.Core;
using Gremlins.Services;

namespace Gremlins.Tricks;

/// <summary>
/// Silently clears the clipboard at random intervals.
/// You copied something, you go to paste... nothing.
/// </summary>
public class TheAmnesiac : BaseGremlin
{
    public TheAmnesiac(ExecutionGate gate) : base(gate) { }

    public override string Id          => "the_amnesiac";
    public override string Name        => "The Amnesiac";
    public override string Description => "Randomly clears your clipboard. You copied that, right? Are you sure?";
    public override string Emoji       => "🧠";

    protected override async Task RunLoopAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var intervalMs = Severity switch
            {
                Severity.Mischievous => RandomBetween(10 * 60_000, 20 * 60_000),
                Severity.Annoying    => RandomBetween(4 * 60_000, 8 * 60_000),
                Severity.Unhinged    => RandomBetween(60_000, 3 * 60_000),
                _                    => 10 * 60_000
            };

            intervalMs = ApplyIdleBoost(intervalMs);
            await Task.Delay(intervalMs, ct);
            if (ct.IsCancellationRequested) break;

            if (!Gate.ShouldExecute())
                continue;

            // Clipboard must be accessed on STA thread
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                try { System.Windows.Clipboard.Clear(); }
                catch { /* clipboard might be locked by another app */ }
            });
            Gate.LogGremlin(Name, "cleared clipboard");
        }
    }
}
