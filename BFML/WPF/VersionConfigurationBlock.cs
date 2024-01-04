﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using BFML.Core;
using BFML.Repository;
using CmlLib.Core;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;
using CmlLib.Core.VersionMetadata;
using Utils;
using Version = Utils.Version;

namespace BFML.WPF;

internal sealed class VersionConfigurationBlock
{
    public event Action Changed;
    public bool IsModded => _isModdedToggle.IsChecked!.Value;

    public Option<Version> VanillaVersion => String.IsNullOrEmpty(_vanillaVersions.Text)
        ? Option<Version>.None
        : Option<Version>.Some(new Version(_vanillaVersions.Text));

    public Option<Forge> Forge => !IsModded || String.IsNullOrEmpty(_forgeVersions.Text)
        ? Option<Forge>.None
        : Option<Forge>.Some(_repo.LoadForgeVersions(VanillaVersion.Value).Result
            .First(forge => forge.SubVersion.ToString() == _forgeVersions.Text));
        
    public ModPack ModPack { get; set; }

    private readonly ToggleButton _isModdedToggle;
    private readonly ComboBox _vanillaVersions;
    private readonly StackPanel _forgeLine;
    private readonly ComboBox _forgeVersions;
    private readonly StackPanel _modPackLine;
    private readonly ComboBox _modPacks;

    private readonly Game _game;
    private readonly Repo _repo;
    
    public VersionConfigurationBlock(ToggleButton isModdedToggle, ComboBox vanillaVersions, ComboBox forgeVersions, ComboBox modPacks, StackPanel forgeLine, StackPanel modPackLine, Game game, Repo repo)
    {
        _isModdedToggle = isModdedToggle;
        _vanillaVersions = vanillaVersions;
        _forgeVersions = forgeVersions;
        _modPacks = modPacks;
        _forgeLine = forgeLine;
        _modPackLine = modPackLine;
        _game = game;
        _repo = repo;
    }

    public void Start()
    {
        _isModdedToggle.Click += OnModdedToggleToggle;
        _vanillaVersions.DropDownClosed += OnVanillaChanged;
        Changed += UpdateForgeItems;
        OnModdedToggleToggle(null, null);

        MojangVersionLoader remoteVersionLoader = new MojangVersionLoader();
        MVersionCollection remoteVersions = remoteVersionLoader.GetVersionMetadatas();
        foreach (MVersionMetadata version in remoteVersions.Where(version => version.MType == MVersionType.Release))
        {
            _vanillaVersions.Items.Add(version.Name);
        }
        
        Changed?.Invoke();
    }

    private void OnVanillaChanged(object sender, EventArgs e) => Changed?.Invoke();

    private void OnModdedToggleToggle(object sender, RoutedEventArgs e)
    {
        bool isModdedNext = _isModdedToggle.IsChecked!.Value;

        _forgeLine.Visibility = isModdedNext ? Visibility.Visible : Visibility.Collapsed;
        _modPackLine.Visibility = isModdedNext ? Visibility.Visible : Visibility.Collapsed;
        
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
}