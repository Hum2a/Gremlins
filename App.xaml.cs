using System.Windows;
using System.Windows.Controls;
using Gremlins.Core;
using Gremlins.Services;
using Gremlins.Services.Themes;
using Gremlins.UI;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.DependencyInjection;

namespace Gremlins;

public partial class App : System.Windows.Application
{
    /// <summary>DI container after startup (for dialogs that need services).</summary>
    public static IServiceProvider? Services { get; private set; }

    private TaskbarIcon? _trayIcon;
    private ServiceProvider? _services;

    public App()
    {
        InitializeComponent();
        // Brushes must exist before styles resolve; ThemeService replaces this slot on startup.
        Resources.MergedDictionaries.Insert(0, ThemeResourceDictionaryFactory.Create(ThemePalettes.Dark));
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Resources["GremlinTitleIcon"] = IconGenerator.CreateWindowIconSource();

        var services = new ServiceCollection();
        ConfigureServices(services);
        _services = services.BuildServiceProvider();
        Services = _services;

        _services.GetRequiredService<PreferencesService>().Load();
        _services.GetRequiredService<ThemeService>().Initialize();
        _services.GetRequiredService<ExecutionGate>().StartMonitoring();

        // Boot the gremlin engine
        var engine = _services.GetRequiredService<GremlinEngine>();
        engine.Initialise();

        // Set up tray icon
        _trayIcon = new TaskbarIcon
        {
            ToolTipText = "Gremlins — running locally. Right-click for dashboard, Panic, Resume, or Exorcise.",
            ContextMenu = BuildTrayMenu(),
        };
        _trayIcon.TrayMouseDoubleClick += (_, _) => ShowDashboard();

        // Generate a tray icon programmatically (no .ico file needed)
        _trayIcon.Icon = IconGenerator.CreateGremlinIcon();
    }

    private static void ConfigureServices(ServiceCollection services)
    {
        services.AddSingleton<SettingsService>();
        services.AddSingleton<UiSettingsService>();
        services.AddSingleton<PreferencesService>();
        services.AddSingleton<ActivityLogService>();
        services.AddSingleton<ExecutionGate>();
        services.AddSingleton<ThemeService>();
        services.AddSingleton<GremlinEngine>();
        services.AddSingleton<MainViewModel>();

        services.AddSingleton<Tricks.TheDrifter>();
        services.AddSingleton<Tricks.TheTypist>();
        services.AddSingleton<Tricks.TheAmnesiac>();
        services.AddSingleton<Tricks.TheCritic>();
        services.AddSingleton<Tricks.ThePhilosopher>();
        services.AddSingleton<Tricks.TheLagGhost>();
        services.AddSingleton<Tricks.TheRearranger>();
    }

    private ContextMenu BuildTrayMenu()
    {
        var menu = new ContextMenu();
        var titleSrc = Resources["GremlinTitleIcon"] as System.Windows.Media.ImageSource;

        var openItem = new MenuItem
        {
            Header = "Open Gremlins",
            ToolTip = "Show or focus the Gremlins dashboard.",
            Icon = TrayMenuImage(titleSrc, 16),
        };
        openItem.Click += (_, _) => ShowDashboard();

        var panicItem = new MenuItem
        {
            Header = "Panic (silence tricks)",
            ToolTip = "Suppress all tricks for the cooldown set under Safety & profiles (toggles unchanged).",
            Icon = TrayMdl2Glyph("\uEA75"),
        };
        panicItem.Click += (_, _) => _services!.GetRequiredService<ExecutionGate>().TriggerPanic();

        var resumeItem = new MenuItem
        {
            Header = "Resume tricks",
            ToolTip = "End Panic early so tricks follow your rules again.",
            Icon = TrayMdl2Glyph("\uE768"),
        };
        resumeItem.Click += (_, _) => _services!.GetRequiredService<ExecutionGate>().ClearPanic();

        var sep1 = new Separator();

        var exitItem = new MenuItem { Header = "Exorcise all gremlins", ToolTip = "Quit Gremlins completely (tray icon disappears)." };
        exitItem.Click += (_, _) => Shutdown();

        menu.Items.Add(openItem);
        menu.Items.Add(panicItem);
        menu.Items.Add(resumeItem);
        menu.Items.Add(sep1);
        menu.Items.Add(exitItem);

        return menu;
    }

    private static System.Windows.Controls.Image TrayMenuImage(System.Windows.Media.ImageSource? src, int size) =>
        new() { Width = size, Height = size, Source = src };

    private static TextBlock TrayMdl2Glyph(string glyph, double fontSize = 15) =>
        new()
        {
            Text = glyph,
            FontFamily = new System.Windows.Media.FontFamily("Segoe MDL2 Assets"),
            FontSize = fontSize,
            VerticalAlignment = VerticalAlignment.Center,
        };

    private MainWindow? _dashboard;

    private void ShowDashboard()
    {
        if (_dashboard == null || !_dashboard.IsLoaded)
        {
            _dashboard = new MainWindow(_services!.GetRequiredService<MainViewModel>());
        }
        _dashboard.Show();
        _dashboard.Activate();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _services?.GetRequiredService<GremlinEngine>().Shutdown();
        _services?.GetService<ExecutionGate>()?.Dispose();
        _services?.GetService<PreferencesService>()?.Dispose();
        _services?.GetService<ThemeService>()?.Dispose();
        _trayIcon?.Dispose();
        _services?.Dispose();
        Services = null;
        base.OnExit(e);
    }
}
