using CommunityToolkit.Mvvm.ComponentModel;
using Gremlins.Services;

namespace Gremlins.Core;

public abstract partial class BaseGremlin : ObservableObject, IGremlin
{
    private CancellationTokenSource? _cts;

    protected readonly ExecutionGate Gate;

    protected readonly PreferencesService Prefs;

    protected BaseGremlin(ExecutionGate gate, PreferencesService prefs)
    {
        Gate = gate;
        Prefs = prefs;
    }

    public abstract string Id { get; }
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract string Emoji { get; }

    [ObservableProperty]
    private bool _isEnabled;

    [ObservableProperty]
    private Severity _severity = Severity.Mischievous;

    partial void OnIsEnabledChanged(bool value)
    {
        if (value) Start();
        else Stop();
    }

    partial void OnSeverityChanged(Severity value)
    {
        ApplySeverity(value);
        if (IsEnabled)
        {
            Stop();
            Start();
        }
    }

    public void Start()
    {
        if (_cts is not null)
            Stop();

        _cts = new CancellationTokenSource();
        _ = RunLoopAsync(_cts.Token);
    }

    public void Stop()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
        OnStopped();
    }

    protected virtual void OnStopped() { }

    public virtual void ApplySeverity(Severity severity) { }

    protected abstract Task RunLoopAsync(CancellationToken ct);

    /// <summary>
    /// Returns a delay in milliseconds scaled to the severity.
    /// Higher severity = shorter intervals = more chaos.
    /// </summary>
    protected int GetIntervalMs(int baseMs)
    {
        return Severity switch
        {
            Severity.Mischievous => baseMs,
            Severity.Annoying    => baseMs / 2,
            Severity.Unhinged    => baseMs / 5,
            _                    => baseMs
        };
    }

    protected static int RandomBetween(int min, int max) =>
        Random.Shared.Next(min, max + 1);

    /// <summary>Shortens waits when idle boost is enabled.</summary>
    protected int ApplyIdleBoost(int intervalMs)
    {
        var div = Gate.IdleIntervalMultiplier();
        return Math.Max(50, (int)(intervalMs / div));
    }
}
