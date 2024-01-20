using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BFML._3D;
using OpenTK.Wpf;
using System.Windows.Input;
using BFML.Core;
using BFML.Repository;
using CmlLib.Utils;
using Common;
using Utils.Async;

namespace BFML.WPF;

public sealed partial class ManualModeWindow : IDisposable
{
    private SkinPreviewRenderer _skinPreviewRenderer;
    private readonly Game _game;
    private readonly ManualModeRepo _repo;
    private readonly SettingsTab _settingsTab;
    private readonly LoadingScreen _loadingScreen;
    private readonly VersionConfigurationBlock _versionBlock;

    internal ManualModeWindow(ManualModeRepo repo)
    {
        _repo = repo;
        _game = new Game(_repo);
        
        InitializeComponent();
        
        _loadingScreen = new LoadingScreen(Loading, ProgressBar, ProgressText);
        _versionBlock = new VersionConfigurationBlock(
            IsModded, MinecraftVersion,
            ForgeVersion, ModPack,
            ForgeVersionLine, ModPackSelectionLine,
            ForgeAddButton, ForgeRemoveButton, ModPackAddButton, ModPackRemoveButton,
            _game, _repo);
        _settingsTab = new SettingsTab(
            _repo, SettingsTab, MinecraftPathButton, MinecraftPathText,
            JavaPathButton, JavaPathText, FilesValidateMode, RamSlider, Fullscreen, Snapshots);

        Nickname.Text = _repo.LocalPrefs.Nickname;
        _versionBlock = new VersionConfigurationBlock(IsModded, MinecraftVersion, ForgeVersion, 
            ModPack, ForgeVersionLine, ModPackSelectionLine,
            ForgeAddButton, ForgeRemoveButton, ModPackAddButton, ModPackRemoveButton, _game, _repo);
        
        _skinPreviewRenderer = SetUpSkinRenderer();
        Loaded += OnWindowLoaded;
        Nickname.TextChanged += NicknameOnTextChanged;
        _versionBlock.Changed += OnVersionChanged;
    }

    private async void OnVersionChanged()
    {
        try
        {
            Changelogs changelogs = await Changelogs.GetChangelogs();
            string changelogHtml = await changelogs.GetChangelogHtml(_versionBlock.VanillaVersion.Value.ToString());
            Changelog.Text = Regex.Replace(changelogHtml, "<.*?>", String.Empty);
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
            throw;
        }
    }

    private void NicknameOnTextChanged(object sender, TextChangedEventArgs e)
    {
        LocalPrefs localPrefs = _repo.LocalPrefs;
        localPrefs.Nickname = Nickname.Text;
        _repo.SaveLocalPrefs(localPrefs).FireAndForget();
    }

    private async void OnWindowLoaded(object sender, RoutedEventArgs args)
    {
        Loaded -= OnWindowLoaded;
        _versionBlock.Start();
        _settingsTab.Start();

        try
        {
            _skinPreviewRenderer.SetUp(
                (await _repo.LoadDefaultSkin()).Texture, 
                await _repo.LoadShadowTexture());
        }
        catch { }
    }
    
    public void Dispose()
    {
        Nickname.TextChanged -= NicknameOnTextChanged;
        _versionBlock.Changed -= OnVersionChanged;
        _settingsTab?.Dispose();
        _versionBlock?.Dispose();
    }

    private async Task LaunchGame()
    {
        PlayButton.IsEnabled = false;

        GameConfiguration configuration = new GameConfiguration(
            _versionBlock.IsModded,
            _versionBlock.VanillaVersion.Value,
            _versionBlock.Forge.Value,
            _versionBlock.ModPack);
        
        await _game.Launch(_repo.LocalPrefs.Nickname, configuration);
        
        await Task.Delay(10000);
        Close();
    }

    #region PlayerModelRendering

    private SkinPreviewRenderer SetUpSkinRenderer()
    {
        try
        {
            GLWpfControlSettings settings = new GLWpfControlSettings
            {
                MajorVersion = 4,
                MinorVersion = 0
            };
            OpenTkControl.Start(settings);
            return new SkinPreviewRenderer();
        }
        catch (Exception e)
        {
            return null;
        }
    }
    
    private void OnSkinFileDrop(object sender, DragEventArgs e)
    {
        throw new NotImplementedException();

        /*if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

        string[] files = (string[])e.Data.GetData(DataFormats.FileDrop)!;
        if (files.Length != 1 || !files[0].EndsWith(".png")) return;

        FileInfo fileInfo = new FileInfo(files[0]);
        Skin skin = Skin.FromFile(fileInfo);
        //TODO rewrite saved skin
        _skinPreviewRenderer.ChangeSkin(skin);*/
    }
    
    private void SkinPreviewOnRender(TimeSpan obj)
    {
        _skinPreviewRenderer.Render((float)SkinSlider.Value);
    }

    #endregion

    #region Navigation

    private void MoveWindow(object sender, MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed) return;
        
        if (WindowState == WindowState.Maximized) 
        {
            WindowState = WindowState.Normal;
            Application.Current.MainWindow!.Top = 3;
        }
        DragMove();
    }

    private void ShutDown(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispose();
            Application.Current.Shutdown();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
    private void Minimize(object sender, RoutedEventArgs e)
    {
        try
        {
            WindowState = WindowState.Minimized;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    #endregion

    #region InputHandlers

    private void OnPlayButton(object sender, RoutedEventArgs e)
    {
        LaunchGame().FireAndForget();
    }
    
    private void OnReloadFiles(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(new NotImplementedException().Message, "Not quite there yet");
    }

    private void ExitAccount(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(new NotImplementedException().Message, "Not quite there yet");
    }

    private void OnOpenFolderButton(object sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo(_repo.LocalPrefs.GameDirectory.FullName) { UseShellExecute = true });
    }

    #endregion
}