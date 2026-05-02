using System.IO;
using Microsoft.Win32;

namespace Gremlins.Services;

public static class RegistryStartupHelper
{
    private const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string AppName = "Gremlins";

    public static bool IsEnabled()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKey, false);
        return key?.GetValue(AppName) != null;
    }

    /// <summary>
    /// Registers Gremlins in HKCU Run. Optional <paramref name="delaySeconds"/> waits before launch (boot-friendly).
    /// </summary>
    public static void SetEnabled(bool enable, int delaySeconds = 0)
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKey, true);
            if (key is null)
                return;

            if (!enable)
            {
                key.DeleteValue(AppName, throwOnMissingValue: false);
                return;
            }

            var exePath = Environment.ProcessPath;
            if (string.IsNullOrEmpty(exePath))
                exePath = Path.Combine(AppContext.BaseDirectory, "Gremlins.exe");

            delaySeconds = Math.Clamp(delaySeconds, 0, 600);

            string value;
            if (delaySeconds <= 0)
            {
                value = '"' + exePath.Replace("\"", "\\\"") + '"';
            }
            else
            {
                var lit = exePath.Replace("'", "''");
                value =
                    "powershell.exe -NoProfile -WindowStyle Hidden -Command \"Start-Sleep -Seconds " +
                    delaySeconds +
                    "; Start-Process -LiteralPath '" +
                    lit +
                    "'\"";
            }

            key.SetValue(AppName, value);
        }
        catch
        {
            /* registry failures should not crash the app */
        }
    }
}
