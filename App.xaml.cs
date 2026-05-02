using System.Windows;
using Gremlins.Core;
using Gremlins.Services;
using Gremlins.UI;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.DependencyInjection;

namespace Gremlins;

public partial class App : Application
{
    private TaskbarIcon? _trayIcon;
    private ServiceProvider? _services;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        ConfigureServices(services);
        _services = services.BuildServiceProvider();

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
        services.AddSingleton<GremlinEngine>();
        services.AddSingleton<MainViewModel>();

        // Register all gremlins
        services.AddSingleton<Gremlins.Gremlins.TheDrifter>();
        services.AddSingleton<Gremlins.Gremlins.TheTypist>();
        services.AddSingleton<Gremlins.Gremlins.TheAmnesiac>();
        services.AddSingleton<Gremlins.Gremlins.TheCritic>();
        services.AddSingleton<Gremlins.Gremlins.ThePhilosopher>();
        services.AddSingleton<Gremlins.Gremlins.TheLagGhost>();
        services.AddSingleton<Gremlins.Gremlins.TheRearranger>();
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
        _trayIcon?.Dispose();
        _services?.Dispose();
        base.OnExit(e);
    }
}
