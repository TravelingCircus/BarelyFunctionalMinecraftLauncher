using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using BFML.Core;
using BFML.Repository;
using Utils.Async;

namespace BFML.WPF;

internal sealed class SettingsTab : IDisposable
{
    private readonly Repo _repo;
    private readonly TabItem _tab;
    private readonly Slider _ramSlider;
    private readonly ToggleButton _snapshots;
    private readonly ToggleButton _fullscreen;
    private readonly ComboBox _filesValidateMode;
    private readonly Button _minecraftPathButton;
    private readonly TextBlock _minecraftPathText;

    internal SettingsTab(
        Repo repo, TabItem tab, Button minecraftPathButton, TextBlock minecraftPathText, 
        ComboBox filesValidateMode, Slider ramSlider, ToggleButton fullscreen, ToggleButton snapshots)
    {
        _repo = repo;
        _tab = tab;
        _ramSlider = ramSlider;
        _snapshots = snapshots;
        _fullscreen = fullscreen;
        _filesValidateMode = filesValidateMode;
        _minecraftPathText = minecraftPathText;
        _minecraftPathButton = minecraftPathButton;

        _filesValidateMode.ItemsSource = Enum.GetValues<FileValidation>();
        
        _ramSlider.ValueChanged += RamSliderOnValueChanged;
        _snapshots.Click += SnapshotsOnClicked;
        _fullscreen.Click += FullscreenOnClicked;
        _filesValidateMode.SelectionChanged += FilesValidateModeOnSelectionChanged;
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
    }

    private void RamSliderOnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        LocalPrefs localPrefs = _repo.LocalPrefs;
        Slider slider = (Slider)sender;
        localPrefs.DedicatedRam = (int)e.NewValue;
        _repo.SaveLocalPrefs(localPrefs).FireAndForget(_ => slider.Value = e.OldValue);
    }

    private void SnapshotsOnClicked(object sender, RoutedEventArgs e)
    {
        LocalPrefs localPrefs = _repo.LocalPrefs;
        ToggleButton toggle = (ToggleButton)sender;
        localPrefs.ShowSnapshots = toggle.IsChecked.Value;
        _repo.SaveLocalPrefs(localPrefs).FireAndForget(_ => toggle.IsChecked = !toggle.IsChecked);
    }

    private void FullscreenOnClicked(object sender, RoutedEventArgs e)
    {
        LocalPrefs localPrefs = _repo.LocalPrefs;
        ToggleButton toggle = (ToggleButton)sender;
        localPrefs.IsFullscreen = toggle.IsChecked.Value;
        _repo.SaveLocalPrefs(localPrefs).FireAndForget(_ => toggle.IsChecked = !toggle.IsChecked);
    }

    private void FilesValidateModeOnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        LocalPrefs localPrefs = _repo.LocalPrefs;
        ComboBox comboBox = (ComboBox)sender;
        localPrefs.FileValidationMode = (FileValidation)e.AddedItems[0]!;
        _repo.SaveLocalPrefs(localPrefs).FireAndForget(_ => comboBox.SelectedValue = (FileValidation)e.RemovedItems[0]!);
    }
}