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
    string Emoji { get; }
    bool IsEnabled { get; set; }
    Severity Severity { get; set; }

    void Start();
    void Stop();
    void OnSeverityChanged(Severity severity);
}
