using Gremlins.Core;
using Newtonsoft.Json;
using System.IO;

namespace Gremlins.Services;

public class GremlinState
{
    public bool IsEnabled { get; set; }
    public Severity Severity { get; set; }
}

public class SettingsService
{
    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Gremlins",
        "settings.json"
    );

    public Dictionary<string, GremlinState> Load()
    {
        try
        {
            if (!File.Exists(SettingsPath)) return [];
            var json = File.ReadAllText(SettingsPath);
            return JsonConvert.DeserializeObject<Dictionary<string, GremlinState>>(json) ?? [];
        }
        catch
        {
            return [];
        }
    }

    public void Save(Dictionary<string, GremlinState> states)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
            File.WriteAllText(SettingsPath, JsonConvert.SerializeObject(states, Formatting.Indented));
        }
        catch { /* silently fail — don't crash the app over settings */ }
    }
}
