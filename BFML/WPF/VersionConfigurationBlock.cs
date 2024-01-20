using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using BFML.Core;
using BFML.Repository;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;
using Utils;
using Utils.Async;
using Version = Utils.Version;

namespace BFML.WPF;

internal sealed class VersionConfigurationBlock : IDisposable
{
    public event Action Changed;
    public bool IsModded => _isModdedToggle.IsChecked!.Value;

    public Option<Version> VanillaVersion => string.IsNullOrEmpty((string)_vanillaVersions.SelectedItem)
        ? Option<Version>.None
        : Option<Version>.Some(new Version((string)_vanillaVersions.SelectedItem));

    public Option<Forge> Forge => !IsModded || string.IsNullOrEmpty((string)_forgeVersions.SelectedItem)
        ? Option<Forge>.None
        : Option<Forge>.Some(_repo.LoadForgeVersions(VanillaVersion.Value).Result
            .First(forge => forge.SubVersion == new Version((string)_vanillaVersions.SelectedItem)));
        
    public ModPack ModPack { get; set; }
    
    private readonly Game _game;
    private readonly Repo _repo;
    
    private readonly Grid _forgeLine;
    private readonly Grid _modPackLine;
    private readonly ComboBox _modPacks;
    private readonly ComboBox _forgeVersions;
    private readonly ComboBox _vanillaVersions;
    private readonly ToggleButton _isModdedToggle;
    
    public VersionConfigurationBlock(
        Game game, Repo repo, ToggleButton isModdedToggle,
        ComboBox vanillaVersions, ComboBox forgeVersions, ComboBox modPacks,
        Grid forgeLine, Grid modPackLine)
    {
        _game = game;
        _repo = repo;
        _modPacks = modPacks;
        _forgeLine = forgeLine;
        _modPackLine = modPackLine;
        _forgeVersions = forgeVersions;
        _isModdedToggle = isModdedToggle;
        _vanillaVersions = vanillaVersions;
    }

    public void Start()
    {
        Changed += UpdateForgeItems;
        _isModdedToggle.Click += OnModdedToggleClicked;
        _vanillaVersions.SelectionChanged += OnVanillaChanged;

        MojangVersionLoader remoteVersionLoader = new MojangVersionLoader();
        MVersionCollection remoteVersions = remoteVersionLoader.GetVersionMetadatas();
        _vanillaVersions.ItemsSource = remoteVersions
            .Where(version => version.MType == MVersionType.Release)
            .Select(version => version.Name);

        _vanillaVersions.SelectedItem = _repo.LocalPrefs.LastVanillaVersion;
        _isModdedToggle.IsChecked = _repo.LocalPrefs.IsModded;

        UpdateModdedSectionVisibility();
        Changed?.Invoke();
    }
    
    public void Dispose()
    {
        Changed -= UpdateForgeItems;
        _isModdedToggle.Click -= OnModdedToggleClicked;  
        _vanillaVersions.SelectionChanged -= OnVanillaChanged;
    }

    private void OnModdedToggleClicked(object sender, RoutedEventArgs e)
    {
        LocalPrefs localPrefs = _repo.LocalPrefs;
        ToggleButton toggle = (ToggleButton)sender;
        localPrefs.IsModded = toggle.IsChecked.Value;
        _repo.SaveLocalPrefs(localPrefs).FireAndForget();

        UpdateModdedSectionVisibility();
        Changed?.Invoke();
    }

    private async void OnVanillaChanged(object sender, SelectionChangedEventArgs e)
    {
        LocalPrefs localPrefs = _repo.LocalPrefs;
        ComboBox comboBox = (ComboBox)sender;
        localPrefs.LastVanillaVersion = (string)e.AddedItems[0]!;
        bool success = await _repo.SaveLocalPrefs(localPrefs);
        if (!success || !_repo.Validate())
        {
            comboBox.SelectedValue = (FileValidation)e.RemovedItems[0]!;
            return;
        }
        Changed?.Invoke();
    }

    private void UpdateForgeItems()
    {
        Option<Version> vanillaOption = VanillaVersion;
        if (!IsModded || !vanillaOption.IsSome) return;
        _forgeVersions.Items.Clear();
        
        Forge[] versions = _repo.LoadForgeVersions(vanillaOption.Value).Result;

        if (versions.Length < 1)
        {
            _forgeVersions.Items.Add("None");
            _forgeVersions.Text = "None";
            return;
        }
        
        foreach (Forge version in versions)
        {
            _forgeVersions.Items.Add(version.SubVersion.ToString());
        }
        _forgeVersions.Text = versions[0].SubVersion.ToString();
    }

    private void UpdateModdedSectionVisibility()
    {
        bool isModded = _isModdedToggle.IsChecked!.Value;
        _forgeLine.Visibility = isModded ? Visibility.Visible : Visibility.Collapsed;
        _modPackLine.Visibility = isModded ? Visibility.Visible : Visibility.Collapsed;
    }
}