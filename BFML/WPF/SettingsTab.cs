using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using BFML.Core;
using BFML.Repository;
using Microsoft.WindowsAPICodePack.Dialogs;
using Utils.Async;

namespace BFML.WPF;

internal sealed class SettingsTab : IDisposable
{
    private readonly Repo _repo;
    private readonly TabItem _tab;
    private readonly Slider _ramSlider;
    private readonly Button _javaPathButton;
    private readonly TextBlock _javaPathText;
    private readonly ToggleButton _snapshots;
    private readonly ToggleButton _fullscreen;
    private readonly ComboBox _filesValidateMode;
    private readonly Button _minecraftPathButton;
    private readonly TextBlock _minecraftPathText;

    internal SettingsTab(
        Repo repo, TabItem tab, Button minecraftPathButton, TextBlock minecraftPathText,
        Button javaPathButton, TextBlock javaPathText, ComboBox filesValidateMode,
        Slider ramSlider, ToggleButton fullscreen, ToggleButton snapshots)
    {
        _repo = repo;
        _tab = tab;
        _ramSlider = ramSlider;
        _snapshots = snapshots;
        _fullscreen = fullscreen;
        _javaPathText = javaPathText;
        _javaPathButton = javaPathButton;
        _filesValidateMode = filesValidateMode;
        _minecraftPathText = minecraftPathText;
        _minecraftPathButton = minecraftPathButton;

        _filesValidateMode.ItemsSource = Enum.GetValues<FileValidation>();
        
        _ramSlider.ValueChanged += RamSliderOnValueChanged;
        _snapshots.Click += SnapshotsOnClicked;
        _fullscreen.Click += FullscreenOnClicked;
        _filesValidateMode.SelectionChanged += FilesValidateModeOnSelectionChanged;
        _minecraftPathButton.Click += MinecraftPathButtonOnClick;
        _javaPathButton.Click += JavaPathButtonOnClick;
    }

    internal void Start()
    {
        LocalPrefs localPrefs = _repo.LocalPrefs;
        _ramSlider.Value = localPrefs.DedicatedRam;
        _snapshots.IsChecked = localPrefs.ShowSnapshots;
        _fullscreen.IsChecked = localPrefs.IsFullscreen;
        _filesValidateMode.SelectedValue = localPrefs.FileValidationMode;
        _minecraftPathText.Text = localPrefs.GameDirectory.FullName;
    }

    public void Dispose()
    {
        _ramSlider.ValueChanged -= RamSliderOnValueChanged;
        _snapshots.Checked -= SnapshotsOnClicked;
        _fullscreen.Checked -= FullscreenOnClicked;
        _filesValidateMode.SelectionChanged -= FilesValidateModeOnSelectionChanged;
        _minecraftPathButton.Click -= MinecraftPathButtonOnClick;
        _javaPathButton.Click -= JavaPathButtonOnClick;
    }

    private void RamSliderOnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        LocalPrefs localPrefs = _repo.LocalPrefs;
        Slider slider = (Slider)sender;
        localPrefs.DedicatedRam = (int)e.NewValue;
        SaveLocalPrefs(_repo, localPrefs, () => slider.Value = e.OldValue)
            .FireAndForget();
    }

    private void SnapshotsOnClicked(object sender, RoutedEventArgs e)
    {
        LocalPrefs localPrefs = _repo.LocalPrefs;
        ToggleButton toggle = (ToggleButton)sender;
        localPrefs.ShowSnapshots = toggle.IsChecked.Value;
        SaveLocalPrefs(_repo, localPrefs, () => toggle.IsChecked = !toggle.IsChecked)
            .FireAndForget();
    }

    private void FullscreenOnClicked(object sender, RoutedEventArgs e)
    {
        LocalPrefs localPrefs = _repo.LocalPrefs;
        ToggleButton toggle = (ToggleButton)sender;
        localPrefs.IsFullscreen = toggle.IsChecked.Value;
        _repo.SaveLocalPrefs(localPrefs).FireAndForget(_ => toggle.IsChecked = !toggle.IsChecked);
        SaveLocalPrefs(_repo, localPrefs, () => toggle.IsChecked = !toggle.IsChecked)
            .FireAndForget();
    }

    private void FilesValidateModeOnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        LocalPrefs localPrefs = _repo.LocalPrefs;
        ComboBox comboBox = (ComboBox)sender;
        localPrefs.FileValidationMode = (FileValidation)e.AddedItems[0]!;
        SaveLocalPrefs(_repo, localPrefs, () => comboBox.SelectedValue = (FileValidation)e.RemovedItems[0]!)
            .FireAndForget();
    }

    private void MinecraftPathButtonOnClick(object sender, RoutedEventArgs e)
    {
        using CommonOpenFileDialog dialog = new CommonOpenFileDialog();
        dialog.InitialDirectory = _repo.LocalPrefs.GameDirectory.FullName;
        dialog.IsFolderPicker = true;
        
        if (dialog.ShowDialog() != CommonFileDialogResult.Ok) return;

        DirectoryInfo selectedDirectory = new DirectoryInfo(dialog.FileName);
        if(!selectedDirectory.Exists) return;

        LocalPrefs localPrefs = _repo.LocalPrefs;
        DirectoryInfo oldDirectory = localPrefs.GameDirectory;
        
        localPrefs.GameDirectory = selectedDirectory;
        _minecraftPathText.Text = selectedDirectory.FullName;

        SaveLocalPrefs(_repo, localPrefs, () => _minecraftPathText.Text = oldDirectory.FullName)
            .FireAndForget();
    }

    private void JavaPathButtonOnClick(object sender, RoutedEventArgs e)
    {
        using CommonOpenFileDialog dialog = new CommonOpenFileDialog();
        dialog.InitialDirectory = _repo.LocalPrefs.JVMLocation.DirectoryName;
        dialog.IsFolderPicker = false;
        
        if (dialog.ShowDialog() != CommonFileDialogResult.Ok) return;

        FileInfo selectedFile = new FileInfo(dialog.FileName);
        if(selectedFile.Extension != "exe" && selectedFile.Name != "java") return;
        if(!selectedFile.Exists) return;

        LocalPrefs localPrefs = _repo.LocalPrefs;
        FileInfo oldFile = localPrefs.JVMLocation;
        
        localPrefs.JVMLocation = oldFile;
        _javaPathText.Text = selectedFile.FullName;

        SaveLocalPrefs(_repo, localPrefs, () => _javaPathText.Text = oldFile.FullName)
            .FireAndForget();
    }

    private static async Task SaveLocalPrefs(Repo repo, LocalPrefs localPrefs, Action onFailure)
    {
        bool success = await repo.SaveLocalPrefs(localPrefs);
        if (success && repo.Validate()) return;
        
        onFailure?.Invoke();
        MessageBox.Show("Can't save local prefs.");
    }
}