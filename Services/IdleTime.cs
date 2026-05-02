using System.Runtime.InteropServices;

namespace Gremlins.Services;

internal static class IdleTime
{
    [StructLayout(LayoutKind.Sequential)]
    private struct LASTINPUTINFO
    {
        public uint cbSize;
        public uint dwTime;
    }

    [DllImport("user32.dll")]
    private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

    [DllImport("kernel32.dll")]
    private static extern uint GetTickCount();

    /// <summary>Approximate seconds since last keyboard/mouse input.</summary>
    public static int GetIdleSeconds()
    {
        try
        {
            var lii = new LASTINPUTINFO { cbSize = (uint)Marshal.SizeOf<LASTINPUTINFO>() };
            if (!GetLastInputInfo(ref lii))
                return 0;
            var idleTicks = GetTickCount() - lii.dwTime;
            return (int)(idleTicks / 1000);
        }
        catch
        {
            return 0;
        }
    }
}
