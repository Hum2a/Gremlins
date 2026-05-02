using System.IO;
using System.Text;
using Gremlins.Core;
using Gremlins.Services;
using NAudio.Wave;

namespace Gremlins.Tricks;

/// <summary>
/// Monitors the active window title and plays a quietly disappointed noise
/// when you open time-wasting apps. Uses generated tone since we have no .wav file.
/// </summary>
public class TheCritic : BaseGremlin
{
    public TheCritic(ExecutionGate gate, PreferencesService prefs) : base(gate, prefs) { }

    public override string Id          => "the_critic";
    public override string Name        => "The Critic";
    public override string Description => "Lets out a tiny sigh when you open social media or YouTube. It knows.";
    public override string Emoji       => "😔";

    private static readonly string[] ShamefulKeywords =
    [
        "youtube", "twitter", "x.com", "instagram", "tiktok",
        "reddit", "facebook", "netflix", "twitch", "discord",
        "whatsapp", "telegram", "news", "espn", "buzzfeed"
    ];

    private string _lastWindowTitle = "";

    protected override async Task RunLoopAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var pollMs = Prefs.Current.Critic.UseCustomSettings
                ? Math.Clamp(Prefs.Current.Critic.PollIntervalSeconds, 1, 120) * 1000
                : 2000;
            await Task.Delay(ApplyIdleBoost(pollMs), ct).ConfigureAwait(false);

            var hwnd = Win32.GetForegroundWindow();
            var sb = new StringBuilder(512);
            _ = Win32.GetWindowText(hwnd, sb, sb.Capacity);
            var title = sb.ToString().ToLowerInvariant();

            if (title == _lastWindowTitle) continue;
            _lastWindowTitle = title;

            if (TitleMatchesShame(title))
            {
                if (!Gate.ShouldExecute())
                    continue;
                PlaySigh();
                Gate.LogGremlin(Name, "sighed at window title");
            }
        }
    }

    private void PlaySigh()
    {
        var critic = Prefs.Current.Critic;
        float vol = Math.Clamp(critic.SighVolumePercent / 100f, 0.05f, 1f);
        var path = critic.CustomSighSoundPath?.Trim() ?? "";

        if (!string.IsNullOrEmpty(path) && File.Exists(path))
        {
            UiSoundPlayback.PlayFile(path, vol);
            return;
        }

        _ = Task.Run(() => PlayGeneratedSigh(vol));
    }

    /// <summary>Used by settings UI to preview the built-in sigh when no custom file is set.</summary>
    public static void PreviewGeneratedSigh(float volume) => PlayGeneratedSigh(volume);

    private static void PlayGeneratedSigh(float volume)
    {
        try
        {
            const int sampleRate = 44100;
            const double duration = 0.8;
            int samples = (int)(sampleRate * duration);

            using var ms = new MemoryStream();
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write("RIFF"u8.ToArray());
                writer.Write(36 + samples * 2);
                writer.Write("WAVE"u8.ToArray());
                writer.Write("fmt "u8.ToArray());
                writer.Write(16);
                writer.Write((short)1);
                writer.Write((short)1);
                writer.Write(sampleRate);
                writer.Write(sampleRate * 2);
                writer.Write((short)2);
                writer.Write((short)16);
                writer.Write("data"u8.ToArray());
                writer.Write(samples * 2);

                for (int i = 0; i < samples; i++)
                {
                    double t = (double)i / sampleRate;
                    double freq = 300 - (180 * t / duration);
                    double tremolo = 1 + 0.1 * Math.Sin(2 * Math.PI * 6 * t);
                    double envelope = Math.Exp(-3 * t / duration);
                    double sample = 0.15 * tremolo * envelope * Math.Sin(2 * Math.PI * freq * t);
                    writer.Write((short)(sample * short.MaxValue));
                }
            }

            ms.Position = 0;
            using var reader = new WaveFileReader(ms);
            using var output = new WaveOutEvent { Volume = volume };
            output.Init(reader);
            output.Play();
            while (output.PlaybackState == PlaybackState.Playing)
                Thread.Sleep(50);
        }
        catch
        {
            /* audio device might not be available */
        }
    }

    private bool TitleMatchesShame(string title)
    {
        foreach (var k in EnumerateKeywords())
        {
            if (title.Contains(k))
                return true;
        }

        return false;
    }

    private IEnumerable<string> EnumerateKeywords()
    {
        foreach (var k in ShamefulKeywords)
            yield return k;

        var raw = Prefs.Current.Critic.ExtraTitleKeywords;
        if (string.IsNullOrWhiteSpace(raw))
            yield break;

        foreach (var part in raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var s = part.ToLowerInvariant();
            if (s.Length > 0)
                yield return s;
        }
    }
}
