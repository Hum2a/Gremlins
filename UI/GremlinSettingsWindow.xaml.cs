using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Gremlins.Services;
using Gremlins.Tricks;

namespace Gremlins.UI;

public partial class GremlinSettingsWindow : Window
{
    private readonly PreferencesService _preferences;

    public GremlinSettingsWindow(string gremlinId, PreferencesService preferences)
    {
        InitializeComponent();
        _preferences = preferences;
        var vm = new GremlinSettingsViewModel(gremlinId, preferences);
        DataContext = vm;
        Title = vm.WindowTitle;
    }

    private void Done_Click(object sender, RoutedEventArgs e)
    {
        _preferences.SaveImmediate();
        DialogResult = true;
        Close();
    }

    private void BrowseCriticSound_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not GremlinSettingsViewModel vm)
            return;

        var dlg = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "Audio files|*.wav;*.mp3|All files|*.*",
            Title = "Choose The Critic sigh sound",
        };
        if (dlg.ShowDialog() == true)
        {
            vm.Current.Critic.CustomSighSoundPath = dlg.FileName;
            _preferences.SaveImmediate();
        }
    }

    private void ClearCriticSound_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not GremlinSettingsViewModel vm)
            return;

        vm.Current.Critic.CustomSighSoundPath = "";
        _preferences.SaveImmediate();
    }

    private void PreviewCriticSound_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not GremlinSettingsViewModel vm)
            return;

        var c = vm.Current.Critic;
        float vol = Math.Clamp(c.SighVolumePercent / 100f, 0.05f, 1f);
        var path = c.CustomSighSoundPath?.Trim() ?? "";

        if (!string.IsNullOrEmpty(path) && File.Exists(path))
            UiSoundPlayback.PlayFile(path, vol);
        else
            _ = Task.Run(() => TheCritic.PreviewGeneratedSigh(vol));
    }
}
