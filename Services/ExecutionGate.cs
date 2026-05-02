using System.Windows.Threading;
using Gremlins.Core;

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

        if (p.QuietHoursEnabled && InQuietHours())
            return false;

        if (p.ScheduleEnabled && !ScheduleAllows())
            return false;

        if (p.RespectPresentationMode && _presenting)
            return false;

        if (p.RespectFullScreenApps && _fullScreenOrBusy)
            return false;

        if (p.FocusRulesEnabled && IsForegroundDenied())
            return false;

        return true;
    }

    /// <summary>Multiplies effective chaos when idle is long (shortens waits).</summary>
    public double IdleIntervalMultiplier()
    {
        var p = _prefs.Current;
        if (!p.IdleIntensityEnabled)
            return 1.0;
        var idle = IdleTime.GetIdleSeconds();
        if (idle < p.IdleBoostAfterSeconds)
            return 1.0;
        var t = Math.Min(1.0, (idle - p.IdleBoostAfterSeconds) / 600.0);
        return 1.0 + t * 0.45;
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

    private bool InQuietHours()
    {
        var nowMin = DateTime.Now.TimeOfDay.TotalMinutes;
        var start = Math.Clamp(_prefs.Current.QuietStartMinutes, 0, 24 * 60 - 1);
        var end = Math.Clamp(_prefs.Current.QuietEndMinutes, 0, 24 * 60 - 1);
        if (Math.Abs(start - end) < 1)
            return false;
        if (start < end)
            return nowMin >= start && nowMin < end;
        return nowMin >= start || nowMin < end;
    }

    private bool ScheduleAllows()
    {
        var p = _prefs.Current;
        var now = DateTime.Now;
        var dow = now.DayOfWeek;
        var weekend = dow is DayOfWeek.Saturday or DayOfWeek.Sunday;
        if (p.ScheduleWeekdaysOnly && weekend)
            return false;
        if (p.ScheduleWeekendsOnly && !weekend)
            return false;

        var h = now.Hour;
        var from = Math.Clamp(p.ScheduleFromHour, 0, 23);
        var to = Math.Clamp(p.ScheduleToHour, 0, 23);
        if (from == to)
            return true;
        if (from < to)
            return h >= from && h < to;
        return h >= from || h < to;
    }

    private bool IsForegroundDenied()
    {
        var raw = _prefs.Current.FocusDenyExeList;
        if (string.IsNullOrWhiteSpace(raw))
            return false;
        var exe = _foregroundExe;
        if (string.IsNullOrEmpty(exe))
            return false;
        foreach (var part in raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var target = part.ToLowerInvariant();
            if (!target.EndsWith(".exe", StringComparison.Ordinal))
                target += ".exe";
            if (exe.Equals(target, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        _timer.Stop();
    }
}
