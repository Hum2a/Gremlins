using Gremlins.Core;
using System.Runtime.InteropServices;

namespace Gremlins.Gremlins;

/// <summary>
/// Installs a low-level keyboard hook and occasionally substitutes
/// a typed character with a visually similar lookalike.
/// Rare enough that you blame your own fingers.
/// </summary>
public class TheTypist : BaseGremlin
{
    public override string Id          => "the_typist";
    public override string Name        => "The Typist";
    public override string Description => "Occasionally swaps a character you typed for a lookalike. l→I, o→0, etc.";
    public override string Emoji       => "⌨️";

    private static readonly Dictionary<char, char> Lookalikes = new()
    {
        ['l'] = 'I', ['I'] = 'l',
        ['o'] = '0', ['0'] = 'o',
        ['1'] = 'l', ['s'] = 'S',
        ['a'] = 'а', // Cyrillic 'a' — visually identical, functionally different
        ['e'] = 'е', // Cyrillic 'e'
        ['p'] = 'р', // Cyrillic 'r'
    };

    // Probability of substitution per keypress (scales with severity)
    private double SubstitutionChance => Severity switch
    {
        Severity.Mischievous => 0.008, // ~1 in 125 keypresses
        Severity.Annoying    => 0.02,  // ~1 in 50
        Severity.Unhinged    => 0.06,  // ~1 in 17
        _                    => 0.008
    };

    private IntPtr _hookId = IntPtr.Zero;
    private HookProc? _hookProc; // keep delegate alive to prevent GC

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;

    [StructLayout(LayoutKind.Sequential)]
    private struct KBDLLHOOKSTRUCT
    {
        public uint vkCode;
        public uint scanCode;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    protected override Task RunLoopAsync(CancellationToken ct)
    {
        // Install hook on a dedicated STA thread (required for low-level hooks)
        var thread = new Thread(() =>
        {
            _hookProc = HookCallback;
            using var process = System.Diagnostics.Process.GetCurrentProcess();
            using var module = process.MainModule!;
            _hookId = SetWindowsHookEx(WH_KEYBOARD_LL, _hookProc,
                GetModuleHandle(module.ModuleName!), 0);

            // Message pump to keep hook alive
            System.Windows.Forms.Application.Run();
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.IsBackground = true;
        thread.Start();

        // Wait for cancellation then clean up
        ct.Register(() =>
        {
            if (_hookId != IntPtr.Zero)
                UnhookWindowsHookEx(_hookId);
            System.Windows.Forms.Application.ExitThread();
        });

        return Task.CompletedTask;
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && wParam == WM_KEYDOWN)
        {
            var kb = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);
            char c = (char)kb.vkCode;

            if (Lookalikes.TryGetValue(c, out var replacement)
                && Random.Shared.NextDouble() < SubstitutionChance)
            {
                // Suppress original keypress and send the lookalike
                SendCharacter(replacement);
                return (IntPtr)1; // block original key
            }
        }

        return CallNextHookEx(_hookId, nCode, wParam, lParam);
    }

    private static void SendCharacter(char c)
    {
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
                        wScan = c,
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
                        wScan = c,
                        dwFlags = Win32.KEYEVENTF_UNICODE | Win32.KEYEVENTF_KEYUP,
                    }
                }
            }
        };

        Win32.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf<Win32.INPUT>());
    }
}
