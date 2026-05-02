using System.Windows.Threading;

namespace Gremlins.Services;

/// <summary>
/// Central policy: panic cooldown, schedule, quiet hours, presentation/fullscreen, focus deny-list,
/// soft start, and idle-weighted intensity.
/// </summary>
public sealed class ExecutionGate : IDisposable
{
    private readonly PreferencesService _prefs;
    private readonly ActivityLogService _log;
    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(2) };
    private bool _disposed;

    private volatile bool _presenting;
    private volatile bool _fullScreenOrBusy;
    private volatile string _foregroundExe = "";

    public ExecutionGate(PreferencesService prefs, ActivityLogService log)
    {
        _prefs = prefs;
        _log = log;
        _timer.Tick += (_, _) => RefreshSurfaceSignals();
    }

    public DateTime AppStartedUtc { get; } = DateTime.UtcNow;

    public DateTime PanicUntilUtc { get; private set; }

    public void StartMonitoring() => _timer.Start();

    private void RefreshSurfaceSignals()
    {
        _foregroundExe = ForegroundExe.GetForegroundExeName();
        _presenting = PresentationState.IsPresentationMode();
        _fullScreenOrBusy = PresentationState.IsFullScreenOrBusy();
    }

    public bool ShouldExecute()
    {
        var p = _prefs.Current;
        if (DateTime.UtcNow < PanicUntilUtc)
            return false;

        if (p.SoftStartMinutes > 0)
        {
            var softEnd = AppStartedUtc.AddMinutes(p.SoftStartMinutes);
            if (DateTime.UtcNow < softEnd)
                return false;
        }

        var now = DateTime.Now;
        if (p.QuietHoursEnabled &&
            SchedulingRules.IsInQuietHours(p.QuietStartMinutes, p.QuietEndMinutes, now))
            return false;

        if (p.ScheduleEnabled)
        {
            if (!SchedulingRules.ScheduleDayPatternAllows(now.DayOfWeek, p.ScheduleWeekdaysOnly, p.ScheduleWeekendsOnly))
                return false;
            if (!SchedulingRules.ScheduleHourWindowAllows(p.ScheduleFromHour, p.ScheduleToHour, now))
                return false;
        }

        if (p.RespectPresentationMode && _presenting)
            return false;

        if (p.RespectFullScreenApps && _fullScreenOrBusy)
            return false;

        if (p.FocusRulesEnabled &&
            SchedulingRules.ForegroundMatchesDenyList(p.FocusDenyExeList, _foregroundExe))
            return false;

        return true;
    }

    /// <summary>Multiplies effective chaos when idle is long (shortens waits).</summary>
    public double IdleIntervalMultiplier()
    {
        var p = _prefs.Current;
        return IdlePolicy.IntervalMultiplier(p.IdleIntensityEnabled, IdleTime.GetIdleSeconds(), p.IdleBoostAfterSeconds);
    }

    public void TriggerPanic()
    {
        var mins = Math.Clamp(_prefs.Current.PanicCooldownMinutes, 5, 180);
        PanicUntilUtc = DateTime.UtcNow.AddMinutes(mins);
        _log.Add($"Panic: tricks suppressed for ~{mins} minutes (your toggles stay as-is).");
    }

    public void ClearPanic()
    {
        PanicUntilUtc = DateTime.UtcNow.AddSeconds(-1);
        _log.Add("Panic cleared — tricks follow rules again.");
    }

    public void LogGremlin(string gremlinName, string detail)
    {
        if (!_prefs.Current.ActivityLogEnabled)
            return;
        _log.Add($"{gremlinName}: {detail}");
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        _timer.Stop();
    }
}
