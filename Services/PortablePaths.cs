using System.IO;

namespace Gremlins.Services;

/// <summary>
/// When <c>Gremlins.portable</c> or <c>portable.flag</c> exists next to the executable,
/// all app data lives under <see cref="DataDirectory"/> beside the app instead of %AppData%.
/// </summary>
public static class PortablePaths
{
    public const string PortableMarkerFile = "Gremlins.portable";
    public const string PortableMarkerAlt = "portable.flag";

    public static bool IsPortableModeRequested()
    {
        try
        {
            var baseDir = AppContext.BaseDirectory;
            return File.Exists(Path.Combine(baseDir, PortableMarkerFile))
                || File.Exists(Path.Combine(baseDir, PortableMarkerAlt));
        }
        catch
        {
            return false;
        }
    }

    public static string DataDirectory
    {
        get
        {
            if (IsPortableModeRequested())
                return Path.Combine(AppContext.BaseDirectory, "GremlinsData");
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Gremlins");
        }
    }

    public static void EnsureMarkerFileForPortableMode()
    {
        var path = Path.Combine(AppContext.BaseDirectory, PortableMarkerFile);
        File.WriteAllText(path, "Place Gremlins settings next to the executable.\r\nDelete this file to use %AppData% again (after moving data).\r\n");
    }

    public static void RemovePortableMarker()
    {
        try
        {
            File.Delete(Path.Combine(AppContext.BaseDirectory, PortableMarkerFile));
            File.Delete(Path.Combine(AppContext.BaseDirectory, PortableMarkerAlt));
        }
        catch { /* best effort */ }
    }
}
