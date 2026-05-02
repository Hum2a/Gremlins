using Gremlins.Services;
using Xunit;

namespace Gremlins.Tests;

public sealed class UpdateCheckerTests
{
    [Theory]
    [InlineData("1.1.0", "1.0.0", true)]
    [InlineData("v2.0.0", "1.9.9", true)]
    [InlineData("1.0.0", "1.0.1", false)]
    [InlineData("1.0.0", "1.0.0", false)]
    [InlineData(null, "1.0.0", false)]
    [InlineData("", "1.0.0", false)]
    public void Is_newer_than_current(string? remote, string current, bool expected) =>
        Assert.Equal(expected, UpdateChecker.IsNewerThanCurrent(remote, current));
}
