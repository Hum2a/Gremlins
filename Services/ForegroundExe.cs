using System.Diagnostics;
using System.Runtime.InteropServices;
using Gremlins.Core;

namespace Gremlins.Services;

internal static class ForegroundExe
{
    public static string GetForegroundExeName()
    {
        try
        {
            var hwnd = Win32.GetForegroundWindow();
            if (hwnd == IntPtr.Zero)
                return "";

            _ = Win32.GetWindowThreadProcessId(hwnd, out var pid);
            if (pid == 0)
                return "";

            using var p = Process.GetProcessById((int)pid);
            return p.ProcessName + ".exe";
        }
        catch
        {
            return "";
        }
    }
}
