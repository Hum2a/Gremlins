using Newtonsoft.Json;
using System.IO;

namespace Gremlins.Services;

public sealed class PreferencesService : IDisposable
{
    private static readonly JsonSerializerSettings JsonSettings = new()
    {
        Formatting = Formatting.Indented,
        NullValueHandling = NullValueHandling.Ignore,
    };

    private readonly string _path;
    private readonly System.Timers.Timer _debounce = new(400) { AutoReset = false };
    private bool _disposed;

    public Preferences Current { get; private set; } = new();

    public PreferencesService()
    {
        _path = Path.Combine(PortablePaths.DataDirectory, "preferences.json");
        _debounce.Elapsed += (_, _) =>
        {
            try { SaveInternal(); } catch { /* non-fatal */ }
        };
    }

    public void Load()
    {
        try
        {
            Current.PropertyChanged -= OnPreferencePropertyChanged;

            if (!File.Exists(_path))
            {
                Current = new Preferences();
                HookChanges();
                return;
            }

            var json = File.ReadAllText(_path);
            var loaded = JsonConvert.DeserializeObject<Preferences>(json, JsonSettings);
            Current = loaded ?? new Preferences();
            HookChanges();
        }
        catch
        {
            Current.PropertyChanged -= OnPreferencePropertyChanged;
            Current = new Preferences();
            HookChanges();
        }
    }

    private void HookChanges()
    {
        Current.PropertyChanged -= OnPreferencePropertyChanged;
        Current.PropertyChanged += OnPreferencePropertyChanged;
    }

    private void OnPreferencePropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) =>
        DebouncedSave();

    public void DebouncedSave()
    {
        _debounce.Stop();
        _debounce.Start();
    }

    public void SaveImmediate()
    {
        _debounce.Stop();
        SaveInternal();
    }

    private void SaveInternal()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        File.WriteAllText(_path, JsonConvert.SerializeObject(Current, JsonSettings));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _debounce.Stop();
        _debounce.Dispose();
        try { SaveInternal(); } catch { }
    }
}
