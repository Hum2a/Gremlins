using System.Windows;
using System.Windows.Input;
using Gremlins;

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
