using Gremlins.Services;
using Xunit;

namespace Gremlins.Tests;

public sealed class SchedulingRulesTests
{
    [Fact]
    public void Quiet_hours_same_start_end_disables_quiet_block()
    {
        var t = new DateTime(2026, 5, 2, 14, 0, 0);
        Assert.False(SchedulingRules.IsInQuietHours(22 * 60, 22 * 60, t));
    }

    [Fact]
    public void Quiet_hours_same_day_window()
    {
        // 10:00–12:00
        Assert.False(SchedulingRules.IsInQuietHours(10 * 60, 12 * 60, new DateTime(2026, 5, 2, 9, 59, 0)));
        Assert.True(SchedulingRules.IsInQuietHours(10 * 60, 12 * 60, new DateTime(2026, 5, 2, 10, 0, 0)));
        Assert.True(SchedulingRules.IsInQuietHours(10 * 60, 12 * 60, new DateTime(2026, 5, 2, 11, 59, 0)));
        Assert.False(SchedulingRules.IsInQuietHours(10 * 60, 12 * 60, new DateTime(2026, 5, 2, 12, 0, 0)));
    }

    [Fact]
    public void Quiet_hours_overnight_wrap()
    {
        // 22:00–07:00
        Assert.False(SchedulingRules.IsInQuietHours(22 * 60, 7 * 60, new DateTime(2026, 5, 2, 21, 59, 0)));
        Assert.True(SchedulingRules.IsInQuietHours(22 * 60, 7 * 60, new DateTime(2026, 5, 2, 22, 0, 0)));
        Assert.True(SchedulingRules.IsInQuietHours(22 * 60, 7 * 60, new DateTime(2026, 5, 3, 3, 0, 0)));
        Assert.True(SchedulingRules.IsInQuietHours(22 * 60, 7 * 60, new DateTime(2026, 5, 3, 6, 59, 0)));
        Assert.False(SchedulingRules.IsInQuietHours(22 * 60, 7 * 60, new DateTime(2026, 5, 3, 7, 0, 0)));
    }

    [Fact]
    public void Schedule_hour_same_from_to_always_allows()
    {
        var t = new DateTime(2026, 5, 2, 3, 0, 0);
        Assert.True(SchedulingRules.ScheduleHourWindowAllows(17, 17, t));
    }

    [Fact]
    public void Schedule_hour_normal_window()
    {
        Assert.False(SchedulingRules.ScheduleHourWindowAllows(17, 23, new DateTime(2026, 5, 2, 16, 0, 0)));
        Assert.True(SchedulingRules.ScheduleHourWindowAllows(17, 23, new DateTime(2026, 5, 2, 17, 0, 0)));
        Assert.True(SchedulingRules.ScheduleHourWindowAllows(17, 23, new DateTime(2026, 5, 2, 22, 0, 0)));
        Assert.False(SchedulingRules.ScheduleHourWindowAllows(17, 23, new DateTime(2026, 5, 2, 23, 0, 0)));
    }

    [Fact]
    public void Schedule_hour_overnight_window()
    {
        Assert.True(SchedulingRules.ScheduleHourWindowAllows(22, 6, new DateTime(2026, 5, 2, 23, 0, 0)));
        Assert.True(SchedulingRules.ScheduleHourWindowAllows(22, 6, new DateTime(2026, 5, 3, 3, 0, 0)));
        Assert.False(SchedulingRules.ScheduleHourWindowAllows(22, 6, new DateTime(2026, 5, 3, 12, 0, 0)));
    }

    [Fact]
    public void Weekdays_weekends_pattern()
    {
        Assert.True(SchedulingRules.ScheduleDayPatternAllows(DayOfWeek.Wednesday, weekdaysOnly: false, weekendsOnly: false));
        Assert.True(SchedulingRules.ScheduleDayPatternAllows(DayOfWeek.Wednesday, weekdaysOnly: true, weekendsOnly: false));
        Assert.False(SchedulingRules.ScheduleDayPatternAllows(DayOfWeek.Saturday, weekdaysOnly: true, weekendsOnly: false));
        Assert.False(SchedulingRules.ScheduleDayPatternAllows(DayOfWeek.Wednesday, weekdaysOnly: false, weekendsOnly: true));
        Assert.True(SchedulingRules.ScheduleDayPatternAllows(DayOfWeek.Sunday, weekdaysOnly: false, weekendsOnly: true));
    }

    [Fact]
    public void Deny_list_matches_case_insensitive_and_implicit_exe()
    {
        Assert.True(SchedulingRules.ForegroundMatchesDenyList("NOTEPAD", "notepad.exe"));
        Assert.True(SchedulingRules.ForegroundMatchesDenyList("notepad.exe,chrome.exe", "CHROME.EXE"));
        Assert.False(SchedulingRules.ForegroundMatchesDenyList("devenv.exe", "notepad.exe"));
        Assert.False(SchedulingRules.ForegroundMatchesDenyList("", "notepad.exe"));
        Assert.False(SchedulingRules.ForegroundMatchesDenyList("notepad.exe", ""));
    }
}
