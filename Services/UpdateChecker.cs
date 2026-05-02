using System.Net.Http;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace Gremlins.Services;

/// <summary>Optional GitHub Releases latest-version check (no auto-install).</summary>
public sealed class UpdateChecker
{
    private static readonly HttpClient Http = new()
    {
        Timeout = TimeSpan.FromSeconds(12),
    };

    static UpdateChecker()
    {
        Http.DefaultRequestHeaders.UserAgent.ParseAdd("Gremlins/1.0 (update check)");
    }

    /// <returns>Latest tag name from GitHub, or null if skipped / failed.</returns>
    public static async Task<string?> TryGetLatestReleaseTagAsync(string? ownerRepo, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(ownerRepo))
            return null;
        var slash = ownerRepo.IndexOf('/', StringComparison.Ordinal);
        if (slash <= 0 || slash >= ownerRepo.Length - 1)
            return null;
        var owner = ownerRepo[..slash].Trim();
        var repo = ownerRepo[(slash + 1)..].Trim();
        if (owner.Length == 0 || repo.Length == 0)
            return null;

        var url = $"https://api.github.com/repos/{owner}/{repo}/releases/latest";
        try
        {
            var json = await Http.GetStringAsync(url, ct).ConfigureAwait(false);
            var token = JObject.Parse(json);
            return token["tag_name"]?.ToString();
        }
        catch
        {
            return null;
        }
    }

    /// <summary>Compare "v1.2.3" or "1.2.3" style tags to current semantic version.</summary>
    public static bool IsNewerThanCurrent(string? remoteTag, string currentSemantic)
    {
        if (string.IsNullOrWhiteSpace(remoteTag))
            return false;
        var remote = Regex.Replace(remoteTag.Trim(), "^v", "", RegexOptions.IgnoreCase);
        if (!Version.TryParse(NormalizeVersion(remote), out var rv))
            return false;
        if (!Version.TryParse(NormalizeVersion(currentSemantic), out var cv))
            return false;
        return rv > cv;
    }

    private static string NormalizeVersion(string s)
    {
        var parts = s.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 3)
            return $"{parts[0]}.{parts[1]}.{parts[2]}";
        if (parts.Length == 2)
            return $"{parts[0]}.{parts[1]}.0";
        if (parts.Length == 1)
            return $"{parts[0]}.0.0";
        return "0.0.0";
    }
}
