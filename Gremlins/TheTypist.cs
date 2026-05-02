using System.Runtime.InteropServices;
using System.Text;
using Gremlins.Core;
using Gremlins.Services;

namespace Gremlins.Tricks;

/// <summary>
/// Installs a low-level keyboard hook and occasionally substitutes
/// a typed character with a visually similar lookalike.
/// Rare enough that you blame your own fingers.
/// </summary>
public class TheTypist : BaseGremlin
{
    public TheTypist(ExecutionGate gate, PreferencesService prefs) : base(gate, prefs) { }

    public override string Id          => "the_typist";
    public override string Name        => "The Typist";
    public override string Description => "Occasionally swaps a character you typed for a lookalike. l→I, o→0, etc.";
    public override string Emoji       => "⌨️";

    private static readonly Dictionary<char, char> Lookalikes = new()
    {
        ['l'] = 'I', ['I'] = 'l',
        ['o'] = '0', ['0'] = 'o',
        ['1'] = 'l', ['s'] = 'S',
        ['a'] = 'а',
        ['e'] = 'е',
        ['p'] = 'р',
    };

    private double GetSubstitutionChance()
    {
        var t = Prefs.Current.Typist;
        if (t.UseCustomSettings)
            return Math.Clamp(t.SubstitutionChancePercent / 100.0, 0.0005, 0.35);

        return Severity switch
        {
            Severity.Mischievous => 0.008,
            Severity.Annoying    => 0.02,
            Severity.Unhinged    => 0.06,
            _                    => 0.008
        };
    }

    private IntPtr _hookId = IntPtr.Zero;
    private Win32.HookProc? _hookProc;
    private Thread? _hookThread;
    private uint _hookNativeThreadId;

    protected override async Task RunLoopAsync(CancellationToken ct)
    {
        var ready = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        _hookProc = HookCallback;

        _hookThread = new Thread(() =>
        {
            try
            {
                using var process = System.Diagnostics.Process.GetCurrentProcess();
                using var module = process.MainModule!;
                _hookNativeThreadId = Win32.GetCurrentThreadId();
                _hookId = Win32.SetWindowsHookEx(
                    Win32.WH_KEYBOARD_LL,
                    _hookProc,
                    Win32.GetModuleHandle(module.ModuleName),
                    0);

                if (_hookId == IntPtr.Zero)
                {
                    ready.TrySetException(new InvalidOperationException("Keyboard hook installation failed."));
                    return;
                }

                ready.TrySetResult();
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
            Name = "Gremlins-TheTypist-Hook",
        };
        _hookThread.SetApartmentState(ApartmentState.STA);
        _hookThread.Start();

        await ready.Task.ConfigureAwait(false);

        try
        {
            await Task.Delay(Timeout.Infinite, ct).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
        }

        if (_hookNativeThreadId != 0)
            Win32.PostThreadMessage(_hookNativeThreadId, Win32.WM_QUIT, IntPtr.Zero, IntPtr.Zero);

        _hookThread?.Join(TimeSpan.FromSeconds(5));
        _hookThread = null;
        _hookNativeThreadId = 0;
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (!Gate.ShouldExecute())
            return Win32.CallNextHookEx(_hookId, nCode, wParam, lParam);

        if (nCode >= 0 && (wParam == (IntPtr)Win32.WM_KEYDOWN || wParam == (IntPtr)Win32.WM_SYSKEYDOWN))
        {
            var kb = Marshal.PtrToStructure<Win32.KBDLLHOOKSTRUCT>(lParam);

            var kbState = new byte[256];
            Win32.GetKeyboardState(kbState);
            var sb = new StringBuilder(8);
            int conv = Win32.ToUnicode(kb.vkCode, kb.scanCode, kbState, sb, sb.Capacity, 0);
            if (conv == 1 && sb.Length > 0)
            {
                char c = sb[0];
                if (Lookalikes.TryGetValue(c, out var replacement)
                    && Random.Shared.NextDouble() < GetSubstitutionChance())
                {
                    SendCharacter(replacement);
                    Gate.LogGremlin(Name, $"swapped “{c}” → lookalike");
                    return (IntPtr)1;
                }
            }
        }

        return Win32.CallNextHookEx(_hookId, nCode, wParam, lParam);
    }

    private static void SendCharacter(char c)
    {
        ushort u = c;
        var inputs = new Win32.INPUT[]
        {
            new()
            {
                type = Win32.INPUT_KEYBOARD,
                u = new Win32.InputUnion
                {
                    ki = new Win32.KEYBDINPUT
                    {
                        wVk = 0,
                        wScan = u,
                        dwFlags = Win32.KEYEVENTF_UNICODE,
                    }
                }
            },
            new()
            {
                type = Win32.INPUT_KEYBOARD,
                u = new Win32.InputUnion
                {
                    ki = new Win32.KEYBDINPUT
                    {
                        wVk = 0,
                        wScan = u,
                        dwFlags = Win32.KEYEVENTF_UNICODE | Win32.KEYEVENTF_KEYUP,
                    }
                }
            },
        };

        Win32.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf<Win32.INPUT>());
    }
}
