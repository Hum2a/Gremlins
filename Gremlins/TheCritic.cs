using System.IO;
using System.Text;
using Gremlins.Core;
using NAudio.Wave;

namespace Gremlins.Tricks;

/// <summary>
/// Monitors the active window title and plays a quietly disappointed noise
/// when you open time-wasting apps. Uses generated tone since we have no .wav file.
/// </summary>
public class TheCritic : BaseGremlin
{
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
            await Task.Delay(2000, ct).ConfigureAwait(false);

            var hwnd = Win32.GetForegroundWindow();
            var sb = new StringBuilder(512);
            _ = Win32.GetWindowText(hwnd, sb, sb.Capacity);
            var title = sb.ToString().ToLowerInvariant();

            if (title == _lastWindowTitle) continue;
            _lastWindowTitle = title;

            if (ShamefulKeywords.Any(k => title.Contains(k)))
                PlaySigh();
        }
    }

    private void PlaySigh()
    {
        _ = Task.Run(() =>
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
                using var output = new WaveOutEvent { Volume = 0.3f };
                output.Init(reader);
                output.Play();
                while (output.PlaybackState == PlaybackState.Playing)
                    Thread.Sleep(50);
            }
            catch
            {
                /* audio device might not be available */
            }
        });
    }
}
