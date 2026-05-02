using Gremlins.Core;

namespace Gremlins.Services;

internal static class PresentationState
{
    public static bool IsPresentationMode()
    {
        try
        {
            var hr = Win32.SHQueryUserNotificationState(out var q);
            if (hr != 0)
                return false;
            return q == Win32.QUERY_USER_NOTIFICATION_STATE.QUNS_PRESENTATION_MODE;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsFullScreenOrBusy()
    {
        try
        {
            var hr = Win32.SHQueryUserNotificationState(out var q);
            if (hr != 0)
                return false;
            return q is Win32.QUERY_USER_NOTIFICATION_STATE.QUNS_RUNNING_D3D_FULL_SCREEN
                or Win32.QUERY_USER_NOTIFICATION_STATE.QUNS_BUSY;
        }
        catch
        {
            return false;
        }
    }
}
