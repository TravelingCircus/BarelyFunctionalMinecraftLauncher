using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using BFML.Core;
using BFML.Repository;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;
using CmlLib.Core.VersionMetadata;
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
            .First(forge => forge.SubVersion == new Version((string)_forgeVersions.SelectedItem)));
        
    public ModPack ModPack { get; set; }
    
    private readonly Game _game;
    private readonly Repo _repo;
    private readonly Grid _forgeLine;
    private readonly Grid _modPackLine;
    private readonly Button _forgeAddButton;
    private readonly Button _forgeRemoveButton;
    private readonly Button _modPackAddButton;
    private readonly Button _modPackRemoveButton;
    private readonly ComboBox _modPacks;
    private readonly ComboBox _forgeVersions;
    private readonly ComboBox _vanillaVersions;
    private readonly ToggleButton _isModdedToggle;
    
    public VersionConfigurationBlock(ToggleButton isModdedToggle, 
        ComboBox vanillaVersions, 
        ComboBox forgeVersions, 
        ComboBox modPacks, 
        Grid forgeLine, 
        Grid modPackLine,
        Button forgeAddButton,
        Button forgeRemoveButton,
        Button modPackAddButton,
        Button modPackRemoveButton,
        Game game, 
        Repo repo)
    {
        _game = game;
        _repo = repo;
        _modPacks = modPacks;
        _forgeLine = forgeLine;
        _modPackLine = modPackLine;
        _forgeVersions = forgeVersions;
        _isModdedToggle = isModdedToggle;
        _vanillaVersions = vanillaVersions;
        _forgeAddButton = forgeAddButton;
        _forgeRemoveButton = forgeRemoveButton;
        _modPackAddButton = modPackAddButton;
        _modPackRemoveButton = modPackRemoveButton;
    }

    public void Start()
    {
        _isModdedToggle.Click += OnModdedToggleClicked;
        Changed += UpdateForgeItems;
        _vanillaVersions.SelectionChanged += OnVanillaChanged;

        MojangVersionLoader remoteVersionLoader = new MojangVersionLoader();
        MVersionCollection remoteVersions = remoteVersionLoader.GetVersionMetadatas();
        _vanillaVersions.ItemsSource = remoteVersions
            .Where(version => version.MType == MVersionType.Release)
            .Select(version => version.Name);

        _vanillaVersions.SelectedItem = _repo.LocalPrefs.LastVanillaVersion;
        _isModdedToggle.IsChecked = _repo.LocalPrefs.IsModded;

        _forgeAddButton.Click += OnForgeAddClicked;
        _forgeRemoveButton.Click += OnForgeRemoveClicked;
        
        UpdateModdedSectionVisibility();
        Changed?.Invoke();
    }
    
    public void Dispose()
    {
        Changed -= UpdateForgeItems;
        _isModdedToggle.Click -= OnModdedToggleClicked;  
        _vanillaVersions.SelectionChanged -= OnVanillaChanged;
        _forgeAddButton.Click -= OnForgeAddClicked;
        _forgeRemoveButton.Click -= OnForgeRemoveClicked;
    }

    private void OnModdedToggleClicked(object sender, RoutedEventArgs e)
    {
        LocalPrefs localPrefs = _repo.LocalPrefs;
        localPrefs.IsModded = _isModdedToggle.IsChecked!.Value;
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

    private async void OnForgeAddClicked(object sender, RoutedEventArgs args)
    {
        try
        {
            Result<Forge> loadResult = await _repo.AddForgeWithDialogue();
            if (loadResult is { IsOk: false, Error: IOException ioError }) MessageBox.Show(ioError.Message);
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }
        finally
        {
            Changed?.Invoke();
        }
    }

    private async void OnForgeRemoveClicked(object sender, RoutedEventArgs args)
    {
        if (!Forge.IsSome) return;
        
        try
        {
            await _repo.RemoveForge(Forge.Value);
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }
        finally
        {
            Changed?.Invoke();
        }
    }
}