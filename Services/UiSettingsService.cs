using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;

namespace Gremlins.Services;

/// <summary>Persists UI preferences (theme) separate from gremlin engine state.</summary>
public class UiSettingsService
{
    private static string UiPath => Path.Combine(PortablePaths.DataDirectory, "ui.json");

    private static readonly JsonSerializerSettings JsonSettings = new()
    {
        Formatting = Formatting.Indented,
        Converters = { new StringEnumConverter() },
    };

    public UiSettings Load()
    {
        try
        {
            if (!File.Exists(UiPath))
                return new UiSettings();
            var json = File.ReadAllText(UiPath);
            return JsonConvert.DeserializeObject<UiSettings>(json, JsonSettings) ?? new UiSettings();
        }
        catch
        {
            return new UiSettings();
        }
    }

    public void Save(UiSettings settings)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(UiPath)!);
            File.WriteAllText(UiPath, JsonConvert.SerializeObject(settings, JsonSettings));
        }
        catch { /* non-fatal */ }
    }
}
