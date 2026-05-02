using System.Collections.ObjectModel;

namespace Gremlins.Services;

/// <summary>Ring buffer of local-only activity lines for transparency.</summary>
public sealed class ActivityLogService
{
    private const int MaxEntries = 400;

    public ObservableCollection<string> Entries { get; } = [];

    public void Clear()
    {
        var disp = System.Windows.Application.Current?.Dispatcher;
        disp?.Invoke(() => Entries.Clear());
    }

    public void Add(string message)
    {
        var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}  {message}";
        var disp = System.Windows.Application.Current?.Dispatcher;
        if (disp is null)
            return;
        disp.Invoke(() =>
        {
            Entries.Insert(0, line);
            while (Entries.Count > MaxEntries)
                Entries.RemoveAt(Entries.Count - 1);
        });
    }
}
