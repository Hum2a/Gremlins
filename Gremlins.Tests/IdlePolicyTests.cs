using Gremlins.Services;
using Xunit;

namespace Gremlins.Tests;

public sealed class IdlePolicyTests
{
    [Fact]
    public void Disabled_returns_one()
    {
        Assert.Equal(1.0, IdlePolicy.IntervalMultiplier(false, 10_000, 300));
    }

    [Fact]
    public void Below_threshold_returns_one()
    {
        Assert.Equal(1.0, IdlePolicy.IntervalMultiplier(true, 299, 300));
    }

    [Fact]
    public void Scales_up_after_threshold()
    {
        var m = IdlePolicy.IntervalMultiplier(true, 900, 300); // 600s past threshold
        Assert.True(m > 1.0);
        Assert.True(m <= 1.45 + 0.0001);
    }
}
