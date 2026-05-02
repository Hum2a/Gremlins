using System.Windows;
using System.Windows.Input;
using Gremlins;
using Gremlins.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Gremlins.UI;

public partial class MainWindow : Window
{
    private readonly MainViewModel _vm;

    public MainWindow(MainViewModel vm)
    {
        InitializeComponent();
        Icon = IconGenerator.CreateWindowIconSource();
        _vm = vm;
        DataContext = vm;
        vm.Initialise();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (_vm.Behavior.OnboardingComplete)
            return;
        var w = new OnboardingWindow { Owner = this };
        if (w.ShowDialog() != true)
            return;
        _vm.Behavior.OnboardingComplete = true;
        if (App.Services is not null)
            App.Services.GetRequiredService<PreferencesService>().SaveImmediate();
    }

    private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            DragMove();
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Hide(); // don't close — keep running in tray
    }

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        e.Cancel = true; // intercept X button too
        Hide();
    }
}
