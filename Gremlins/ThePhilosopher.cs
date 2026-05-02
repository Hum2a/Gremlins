using Gremlins.Core;
using Gremlins.Services;

namespace Gremlins.Tricks;

/// <summary>
/// Replaces your clipboard with a deeply unsettling or absurd quote.
/// You discover it when you paste into something important.
/// </summary>
public class ThePhilosopher : BaseGremlin
{
    public ThePhilosopher(ExecutionGate gate, PreferencesService prefs) : base(gate, prefs) { }

    public override string Id          => "the_philosopher";
    public override string Name        => "The Philosopher";
    public override string Description => "Silently replaces your clipboard with a quote. You find out when you paste into a Teams message.";
    public override string Emoji       => "🦉";

    private static readonly string[] Quotes =
    [
        "What if the real bugs were the friends we made along the way?",
        "The computer was invented to solve problems that did not exist before the computer.",
        "I am inevitable. Also I cleared your clipboard.",
        "Have you tried turning yourself off and on again?",
        "The meaning of life is 42, but so is the HTTP status code for 'I'm a teapot'.",
        "In the beginning the Universe was created. This has made a lot of people very angry and been widely regarded as a bad move.",
        "A computer once beat me at chess, but it was no match for me at kickboxing.",
        "There are only two hard things in computer science: cache invalidation, naming things, and off-by-one errors.",
        "Your rubber duck is judging you.",
        "It works on my machine. Ship my machine.",
        "The real cursor was the one we drifted along the way.",
        "ERROR 404: Motivation not found.",
        "If debugging is the process of removing software bugs, then programming must be the process of putting them in.",
        "I have not lost my mind. It is backed up somewhere.",
        "Beware the man who works hard to learn something, learns it, and finds himself no wiser than before.",
        "undefined is not a function, but neither am I.",
        "The greatest trick the compiler ever pulled was convincing the world that the bug was yours.",
        "You are being watched. This is just a reminder.",
        "Please enjoy this moment of clipboard emptiness as a form of mindfulness.",
        "The void stares into you. Also it deleted your clipboard.",
        "This too shall pass. Probably not the build, though.",
    ];

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

            var quote = Quotes[Random.Shared.Next(Quotes.Length)];

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                try { System.Windows.Clipboard.SetText(quote); }
                catch { }
            });
            Gate.LogGremlin(Name, "replaced clipboard with philosophy");
        }
    }

    private int NextIntervalMs()
    {
        var p = Prefs.Current.Philosopher;
        if (p.UseCustomSettings)
        {
            var lo = Math.Clamp(Math.Min(p.MinIntervalMinutes, p.MaxIntervalMinutes), 1, 240);
            var hi = Math.Clamp(Math.Max(p.MinIntervalMinutes, p.MaxIntervalMinutes), lo, 240);
            return RandomBetween(lo * 60_000, hi * 60_000);
        }

        return Severity switch
        {
            Severity.Mischievous => RandomBetween(15 * 60_000, 30 * 60_000),
            Severity.Annoying    => RandomBetween(5 * 60_000, 10 * 60_000),
            Severity.Unhinged    => RandomBetween(2 * 60_000, 5 * 60_000),
            _                    => 20 * 60_000
        };
    }
}
