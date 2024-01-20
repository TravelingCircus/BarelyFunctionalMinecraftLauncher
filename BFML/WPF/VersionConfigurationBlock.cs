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
    
    public Option<Forge> Forge => !IsModded || string.IsNullOrEmpty((string)_forgeVersions.SelectedItem) || (string)_forgeVersions.SelectedItem == "None"
        ? Option<Forge>.None
        : Option<Forge>.Some(_repo.LoadForgeVersions(VanillaVersion.Value).Result
            .First(forge => forge.SubVersion == new Version((string)_forgeVersions.SelectedItem)));
    
    public Option<ModPack> ModPack => !IsModded || string.IsNullOrEmpty((string)_modPacks.SelectedItem) || (string)_modPacks.SelectedItem == "None"
        ? Option<ModPack>.None
        : Option<ModPack>.Some(_repo.LoadModPacks(Forge.Value).Result
            .First(modPack => modPack.Name == (string)_modPacks.SelectedItem));
    
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
        MojangVersionLoader remoteVersionLoader = new MojangVersionLoader();
        MVersionCollection remoteVersions = remoteVersionLoader.GetVersionMetadatas();
        _vanillaVersions.ItemsSource = remoteVersions
            .Where(version => version.MType == MVersionType.Release)
            .Select(version => version.Name);

        _vanillaVersions.SelectedItem = _repo.LocalPrefs.LastVanillaVersion;
        _isModdedToggle.IsChecked = _repo.LocalPrefs.IsModded;
        
        Changed += UpdateForgeItems;
        Changed += UpdateModPackItems;
        _isModdedToggle.Click += OnModdedToggleClicked;
        _vanillaVersions.SelectionChanged += OnVanillaChanged;
        
        _forgeAddButton.Click += OnForgeAddClicked;
        _forgeRemoveButton.Click += OnForgeRemoveClicked;
        
        _modPackAddButton.Click += OnModPackAddClicked;
        _modPackRemoveButton.Click += OnModPackRemoveClicked;
        
        UpdateModdedSectionVisibility();
        Changed?.Invoke();
    }
    
    public void Dispose()
    {
        Changed -= UpdateForgeItems;
        Changed -= UpdateModPackItems;
        _isModdedToggle.Click -= OnModdedToggleClicked;
        _vanillaVersions.SelectionChanged -= OnVanillaChanged;
        _forgeAddButton.Click -= OnForgeAddClicked;
        _forgeRemoveButton.Click -= OnForgeRemoveClicked;
        _modPackAddButton.Click -= OnModPackAddClicked;
        _modPackRemoveButton.Click -= OnModPackRemoveClicked;
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

    private void UpdateModPackItems()
    {
        Option<Forge> forgeOption = Forge;
        _modPacks.Items.Clear();
        if (!IsModded || !forgeOption.IsSome) return;
        
        ModPack[] modPacks = _repo.LoadModPacks(forgeOption.Value).Result;
        
        if (modPacks.Length < 1)
        {
            _modPacks.Items.Add("None");
            _modPacks.Text = "None";
            return;
        }
        
        foreach (ModPack modPack in modPacks)
        {
            _modPacks.Items.Add(modPack.Name);
        }
        _modPacks.Text = modPacks[0].Name;
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
            Result<bool> removeResult = await _repo.RemoveForge(Forge.Value);
            if (removeResult is { IsOk: false, Error: IOException ioError }) MessageBox.Show(ioError.Message);
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

    private async void OnModPackAddClicked(object sender, RoutedEventArgs args)
    {
        try
        {
            Result<ModPack> loadResult = await _repo.AddModPackWithDialogue();
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

    private async void OnModPackRemoveClicked(object sender, RoutedEventArgs args)
    {
        if (!ModPack.IsSome) return;
        
        try
        {
            Result<bool> removeResult = await _repo.RemoveModPack(ModPack.Value);
            if (removeResult is { IsOk: false, Error: IOException ioError }) MessageBox.Show(ioError.Message);
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