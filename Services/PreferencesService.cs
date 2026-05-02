using CommunityToolkit.Mvvm.ComponentModel;
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

    private readonly List<ObservableObject> _nestedGremlinSettings = [];

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
            UnsubscribeNestedGremlinSettings();

            if (!File.Exists(_path))
            {
                Current = new Preferences();
                HookChanges();
                return;
            }

            var json = File.ReadAllText(_path);
            var loaded = JsonConvert.DeserializeObject<Preferences>(json, JsonSettings);
            Current = loaded ?? new Preferences();
            EnsureGremlinNestedNotNull();
            HookChanges();
        }
        catch
        {
            Current.PropertyChanged -= OnPreferencePropertyChanged;
            UnsubscribeNestedGremlinSettings();
            Current = new Preferences();
            HookChanges();
        }
    }

    /// <summary>Ensures deserialized preferences never leave gremlin sections null (older JSON files).</summary>
    private void EnsureGremlinNestedNotNull()
    {
        Current.Drifter ??= new DrifterGremlinSettings();
        Current.Typist ??= new TypistGremlinSettings();
        Current.Amnesiac ??= new AmnesiacGremlinSettings();
        Current.Critic ??= new CriticGremlinSettings();
        Current.Philosopher ??= new PhilosopherGremlinSettings();
        Current.LagGhost ??= new LagGhostGremlinSettings();
        Current.Rearranger ??= new RearrangerGremlinSettings();
    }

    private void HookChanges()
    {
        Current.PropertyChanged -= OnPreferencePropertyChanged;
        Current.PropertyChanged += OnPreferencePropertyChanged;
        SubscribeNestedGremlinSettings();
    }

    private void SubscribeNestedGremlinSettings()
    {
        UnsubscribeNestedGremlinSettings();
        void Sub(ObservableObject o)
        {
            o.PropertyChanged += OnNestedGremlinPreferenceChanged;
            _nestedGremlinSettings.Add(o);
        }

        Sub(Current.Drifter);
        Sub(Current.Typist);
        Sub(Current.Amnesiac);
        Sub(Current.Critic);
        Sub(Current.Philosopher);
        Sub(Current.LagGhost);
        Sub(Current.Rearranger);
    }

    private void UnsubscribeNestedGremlinSettings()
    {
        foreach (var o in _nestedGremlinSettings)
            o.PropertyChanged -= OnNestedGremlinPreferenceChanged;
        _nestedGremlinSettings.Clear();
    }

    private void OnNestedGremlinPreferenceChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) =>
        DebouncedSave();

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
