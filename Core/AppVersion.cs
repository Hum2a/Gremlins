using System.Reflection;

namespace Gremlins.Core;

/// <summary>
/// Application semantic version from assembly metadata (matches the csproj Version property).
/// </summary>
public static class AppVersion
{
    public static string SemanticVersion
    {
        get
        {
            var asm = Assembly.GetExecutingAssembly();
            var informational =
                asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            if (string.IsNullOrWhiteSpace(informational))
            {
                var v = asm.GetName().Version;
                return v is null ? "0.0.0" : $"{v.Major}.{v.Minor}.{v.Build}";
            }

            var plus = informational.IndexOf('+', StringComparison.Ordinal);
            return plus >= 0 ? informational[..plus] : informational;
        }
    }
}
