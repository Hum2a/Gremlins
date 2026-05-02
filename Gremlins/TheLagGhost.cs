using Gremlins.Core;
using Gremlins.Services;

namespace Gremlins.Tricks;

/// <summary>
/// Installs a low-level mouse hook that introduces artificial delay during bursts.
/// </summary>
public class TheLagGhost : BaseGremlin
{
    public TheLagGhost(ExecutionGate gate) : base(gate) { }

    public override string Id          => "the_lag_ghost";
    public override string Name        => "The Lag Ghost";
    public override string Description => "Introduces fake input delay in bursts. Feels like your PC is crying.";
    public override string Emoji       => "👻";

    private volatile bool _lagging;

    private IntPtr _hookId = IntPtr.Zero;
    private Win32.HookProc? _hookProc;
    private Thread? _hookThread;
    private uint _hookNativeThreadId;

    protected override async Task RunLoopAsync(CancellationToken ct)
    {
        var hookReady = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        _hookProc = HookCallback;

        _hookThread = new Thread(() =>
        {
            try
            {
                using var process = System.Diagnostics.Process.GetCurrentProcess();
                using var module = process.MainModule!;
                _hookNativeThreadId = Win32.GetCurrentThreadId();
                _hookId = Win32.SetWindowsHookEx(
                    Win32.WH_MOUSE_LL,
                    _hookProc,
                    Win32.GetModuleHandle(module.ModuleName),
                    0);

                if (_hookId == IntPtr.Zero)
                {
                    hookReady.TrySetException(new InvalidOperationException("Mouse hook installation failed."));
                    return;
                }

                hookReady.TrySetResult();
                System.Windows.Forms.Application.Run();
            }
            finally
            {
                if (_hookId != IntPtr.Zero)
                {
                    Win32.UnhookWindowsHookEx(_hookId);
                    _hookId = IntPtr.Zero;
                }
            }
        })
        {
            IsBackground = true,
            Name = "Gremlins-LagGhost-Hook",
        };
        _hookThread.SetApartmentState(ApartmentState.STA);
        _hookThread.Start();

        await hookReady.Task.ConfigureAwait(false);

        try
        {
            while (!ct.IsCancellationRequested)
            {
                var intervalMs = Severity switch
                {
                    Severity.Mischievous => RandomBetween(20 * 60_000, 40 * 60_000),
                    Severity.Annoying    => RandomBetween(8 * 60_000, 15 * 60_000),
                    Severity.Unhinged    => RandomBetween(2 * 60_000, 5 * 60_000),
                    _                    => 30 * 60_000
                };

                intervalMs = ApplyIdleBoost(intervalMs);
                await Task.Delay(intervalMs, ct).ConfigureAwait(false);

                if (!Gate.ShouldExecute())
                    continue;

                int lagDurationMs = Severity switch
                {
                    Severity.Mischievous => 20_000,
                    Severity.Annoying    => 40_000,
                    Severity.Unhinged    => 90_000,
                    _                    => 20_000
                };

                _lagging = true;
                Gate.LogGremlin(Name, "lag burst started");
                try
                {
                    await Task.Delay(lagDurationMs, ct).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                finally
                {
                    _lagging = false;
                }
            }
        }
        finally
        {
            _lagging = false;
            if (_hookNativeThreadId != 0)
                Win32.PostThreadMessage(_hookNativeThreadId, Win32.WM_QUIT, IntPtr.Zero, IntPtr.Zero);

            _hookThread?.Join(TimeSpan.FromSeconds(5));
            _hookThread = null;
            _hookNativeThreadId = 0;
        }
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (!Gate.ShouldExecute())
            return Win32.CallNextHookEx(_hookId, nCode, wParam, lParam);

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

        return Win32.CallNextHookEx(_hookId, nCode, wParam, lParam);
    }
}
