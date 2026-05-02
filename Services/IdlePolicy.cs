namespace Gremlins.Services;

/// <summary>Idle-based interval multiplier (pure math for tests).</summary>
public static class IdlePolicy
{
    public static double IntervalMultiplier(bool idleIntensityEnabled, int idleSeconds, int boostAfterSeconds)
    {
        if (!idleIntensityEnabled)
            return 1.0;
        if (idleSeconds < boostAfterSeconds)
            return 1.0;
        var t = Math.Min(1.0, (idleSeconds - boostAfterSeconds) / 600.0);
        return 1.0 + t * 0.45;
    }
}
