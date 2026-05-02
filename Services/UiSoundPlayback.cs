using System.IO;
using System.Media;
using NAudio.Wave;

namespace Gremlins.Services;

/// <summary>Plays UI toggle sounds and optional WAV/MP3 files using NAudio.</summary>
public static class UiSoundPlayback
{
    /// <summary>Respects <see cref="Preferences.UiSoundsEnabled"/>.</summary>
    public static void PlayToggleSound(Preferences p)
    {
        if (!p.UiSoundsEnabled)
            return;
        PlayToggleSoundCore(p);
    }

    /// <summary>Preview in Settings — plays regardless of the enable checkbox.</summary>
    public static void PreviewToggleSound(Preferences p) => PlayToggleSoundCore(p);

    private static void PlayToggleSoundCore(Preferences p)
    {
        var path = p.UiSoundCustomPath?.Trim() ?? "";
        if (!string.IsNullOrEmpty(path) && File.Exists(path))
        {
            PlayFile(path, Math.Clamp(p.UiSoundVolumePercent / 100f, 0f, 1f));
            return;
        }

        switch (p.UiSoundPreset)
        {
            case UiSoundPreset.None:
                return;
            case UiSoundPreset.Asterisk:
                SystemSounds.Asterisk.Play();
                break;
            case UiSoundPreset.Beep:
                SystemSounds.Beep.Play();
                break;
            case UiSoundPreset.Exclamation:
                SystemSounds.Exclamation.Play();
                break;
            case UiSoundPreset.Hand:
                SystemSounds.Hand.Play();
                break;
            case UiSoundPreset.Question:
                SystemSounds.Question.Play();
                break;
            default:
                SystemSounds.Asterisk.Play();
                break;
        }
    }

    /// <summary>Fire-and-forget playback on a background thread.</summary>
    public static void PlayFile(string path, float volume01)
    {
        _ = Task.Run(() => PlayFileSync(path, volume01));
    }

    /// <summary>Synchronous playback (caller may already be on background thread).</summary>
    public static void PlayFileSync(string path, float volume01)
    {
        try
        {
            using var reader = new AudioFileReader(path) { Volume = volume01 };
            using var output = new WaveOutEvent();
            output.Init(reader);
            output.Play();
            while (output.PlaybackState == PlaybackState.Playing)
                Thread.Sleep(30);
        }
        catch
        {
            /* missing codec, device unavailable */
        }
    }
}
