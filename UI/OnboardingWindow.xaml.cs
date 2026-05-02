using System.Windows;

namespace Gremlins.UI;

public partial class OnboardingWindow : Window
{
    public OnboardingWindow()
    {
        InitializeComponent();
    }

    private void Continue_Click(object sender, RoutedEventArgs e)
    {
        if (Ack.IsChecked != true)
        {
            System.Windows.MessageBox.Show(this, "Please confirm the checkbox first.", "Gremlins",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        DialogResult = true;
        Close();
    }
}
