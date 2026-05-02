using System.Windows;
using Gremlins.Core;
using Gremlins.Services;
using Gremlins.Services.Themes;
using Gremlins.UI;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.DependencyInjection;

namespace Gremlins;

public partial class App : System.Windows.Application
{
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

        var services = new ServiceCollection();
        ConfigureServices(services);
        _services = services.BuildServiceProvider();

        _services.GetRequiredService<ThemeService>().Initialize();

        // Boot the gremlin engine
        var engine = _services.GetRequiredService<GremlinEngine>();
        engine.Initialise();

        // Set up tray icon
        _trayIcon = new TaskbarIcon
        {
            ToolTipText = "Gremlins — running 👹",
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

    private System.Windows.Controls.ContextMenu BuildTrayMenu()
    {
        var menu = new System.Windows.Controls.ContextMenu();

        var openItem = new System.Windows.Controls.MenuItem { Header = "👹  Open Gremlins" };
        openItem.Click += (_, _) => ShowDashboard();

        var separator = new System.Windows.Controls.Separator();

        var exitItem = new System.Windows.Controls.MenuItem { Header = "Exorcise all gremlins" };
        exitItem.Click += (_, _) => Shutdown();

        menu.Items.Add(openItem);
        menu.Items.Add(separator);
        menu.Items.Add(exitItem);

        return menu;
    }

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
        _services?.GetService<ThemeService>()?.Dispose();
        _trayIcon?.Dispose();
        _services?.Dispose();
        base.OnExit(e);
    }
}
