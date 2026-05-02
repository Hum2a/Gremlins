using Gremlins.Core;
using System.Runtime.InteropServices;

namespace Gremlins.Gremlins;

/// <summary>
/// Installs a low-level mouse hook that introduces artificial delay,
/// making the cursor feel like it's running through treacle for ~30 seconds.
/// Feels exactly like your PC is dying.
/// </summary>
public class TheLagGhost : BaseGremlin
{
    public override string Id          => "the_lag_ghost";
    public override string Name        => "The Lag Ghost";
    public override string Description => "Introduces a fake input delay for 30 seconds at random. Feels like your PC is crying.";
    public override string Emoji       => "👻";

    private volatile bool _lagging = false;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, MouseHookProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll")]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll")]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    private delegate IntPtr MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam);

    private const int WH_MOUSE_LL = 14;
    private IntPtr _hookId = IntPtr.Zero;
    private MouseHookProc? _hookProc;

    protected override async Task RunLoopAsync(CancellationToken ct)
    {
        // Install hook on STA thread
        var hookReady = new TaskCompletionSource();
        var thread = new Thread(() =>
        {
            _hookProc = HookCallback;
            using var process = System.Diagnostics.Process.GetCurrentProcess();
            using var module = process.MainModule!;
            _hookId = SetWindowsHookEx(WH_MOUSE_LL, _hookProc,
                GetModuleHandle(module.ModuleName!), 0);
            hookReady.SetResult();
            System.Windows.Forms.Application.Run();
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.IsBackground = true;
        thread.Start();

        await hookReady.Task;

        ct.Register(() =>
        {
            _lagging = false;
            if (_hookId != IntPtr.Zero)
                UnhookWindowsHookEx(_hookId);
            System.Windows.Forms.Application.ExitThread();
        });

        // Main loop: trigger lag bursts at random intervals
        while (!ct.IsCancellationRequested)
        {
            var intervalMs = Severity switch
            {
                Severity.Mischievous => RandomBetween(20 * 60_000, 40 * 60_000),
                Severity.Annoying    => RandomBetween(8 * 60_000, 15 * 60_000),
                Severity.Unhinged    => RandomBetween(2 * 60_000, 5 * 60_000),
                _                    => 30 * 60_000
            };

            await Task.Delay(intervalMs, ct);
            if (ct.IsCancellationRequested) break;

            int lagDuration = Severity switch
            {
                Severity.Mischievous => 20_000,  // 20 seconds
                Severity.Annoying    => 40_000,  // 40 seconds
                Severity.Unhinged    => 90_000,  // 1.5 minutes — pure evil
                _                    => 20_000
            };

            _lagging = true;
            await Task.Delay(lagDuration, ct);
            _lagging = false;
        }
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && _lagging)
        {
            int delayMs = Severity switch
            {
                Severity.Mischievous => RandomBetween(150, 300),
                Severity.Annoying    => RandomBetween(250, 500),
                Severity.Unhinged    => RandomBetween(400, 800),
                _                    => 200
            };
            Thread.Sleep(delayMs);
        }
        return CallNextHookEx(_hookId, nCode, wParam, lParam);
    }
}
