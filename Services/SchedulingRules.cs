namespace Gremlins.Services;

/// <summary>Pure functions for quiet hours, schedule windows, and focus deny-list matching (unit-tested).</summary>
public static class SchedulingRules
{
    public static bool IsInQuietHours(int quietStartMinutes, int quietEndMinutes, DateTime localNow)
    {
        var nowMin = localNow.TimeOfDay.TotalMinutes;
        var start = Math.Clamp(quietStartMinutes, 0, 24 * 60 - 1);
        var end = Math.Clamp(quietEndMinutes, 0, 24 * 60 - 1);
        if (Math.Abs(start - end) < 1)
            return false;
        if (start < end)
            return nowMin >= start && nowMin < end;
        return nowMin >= start || nowMin < end;
    }

    /// <summary>Hour in [<paramref name="fromHour"/>, <paramref name="toHour"/>) with overnight wrap.</summary>
    public static bool ScheduleHourWindowAllows(int fromHour, int toHour, DateTime localNow)
    {
        var h = localNow.Hour;
        var from = Math.Clamp(fromHour, 0, 23);
        var to = Math.Clamp(toHour, 0, 23);
        if (from == to)
            return true;
        if (from < to)
            return h >= from && h < to;
        return h >= from || h < to;
    }

    public static bool ScheduleDayPatternAllows(DayOfWeek dow, bool weekdaysOnly, bool weekendsOnly)
    {
        var weekend = dow is DayOfWeek.Saturday or DayOfWeek.Sunday;
        if (weekdaysOnly && weekend)
            return false;
        if (weekendsOnly && !weekend)
            return false;
        return true;
    }

    /// <returns><see langword="true"/> if foreground exe should block tricks.</returns>
    public static bool ForegroundMatchesDenyList(string rawDenyExeList, string foregroundExeFileName)
    {
        if (string.IsNullOrWhiteSpace(rawDenyExeList))
            return false;
        if (string.IsNullOrEmpty(foregroundExeFileName))
            return false;
        foreach (var part in rawDenyExeList.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var target = part.ToLowerInvariant();
            if (!target.EndsWith(".exe", StringComparison.Ordinal))
                target += ".exe";
            if (foregroundExeFileName.Equals(target, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
