namespace Gremlins.Core;

public enum Severity
{
    Mischievous = 1,
    Annoying = 2,
    Unhinged = 3
}

public interface IGremlin
{
    string Id { get; }
    string Name { get; }
    string Description { get; }
    /// <summary>Single glyph from Segoe MDL2 Assets (Windows).</summary>
    string IconGlyph { get; }
    bool IsEnabled { get; set; }
    Severity Severity { get; set; }

    void Start();
    void Stop();
    void ApplySeverity(Severity severity);
}
