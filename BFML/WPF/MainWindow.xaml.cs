using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BFML._3D;
using OpenTK.Wpf;
using System.Windows.Input;
using BFML.Core;
using CmlLib.Core;
using Common;
using Common.Misc;
using Common.Models;
using XamlRadialProgressBar;

namespace BFML.WPF;

public partial class MainWindow
{
    private Game _game;
    private SkinPreviewRenderer _skinPreviewRenderer;
    private readonly User _user;
    private readonly LocalPrefs _localPrefs;
    private readonly IFileClient _fileClient;
    private readonly LoadingScreen _loadingScreen;
    private readonly LaunchConfiguration _launchConfig;
    private readonly ConfigurationVersion _configVersion;

    public MainWindow(IFileClient fileClient, User user, LocalPrefs localPrefs,
        LaunchConfiguration launchConfig, ConfigurationVersion configVersion)
    {
        InitializeComponent();
        _user = user;
        _fileClient = fileClient;
        _localPrefs = localPrefs;
        _launchConfig = launchConfig;
        _configVersion = configVersion;
        _loadingScreen = new LoadingScreen(Loading, ProgressBar, ProgressText);

        SetUpSkinRenderer();
        Loaded += OnWindowLoaded;
    }

    private async void OnWindowLoaded(object sender, RoutedEventArgs args)
    {
        Loaded -= OnWindowLoaded;
        ApplyLocalPrefs();
        _skinPreviewRenderer.ChangeSkin(_user.SkinPath);
        _game = await Game.SetUp(_fileClient, _launchConfig);
    }

    private async void OnPlayButton(object sender, RoutedEventArgs e)
    {
        if(_game is null) return;
        PlayButton.IsEnabled = false;

        if (!_game.IsReadyToLaunch())
        {
            CompositeProgress progress = _loadingScreen.Show();
            await _game.CleanInstall(progress);
            _loadingScreen.Hide();
        }
        SaveLocalPrefs();
        await _game.Launch((int)RamSlider.Value, FullScreen.IsChecked!.Value, _user.Nickname);
        
        await Task.Delay(10000);
        await _fileClient.TryDispose();
        Close();
    }

    #region PlayerModelRendering

    private void SetUpSkinRenderer()
    {
        GLWpfControlSettings settings = new GLWpfControlSettings
        {
            MajorVersion = 4,
            MinorVersion = 0
        };
        OpenTkControl.Start(settings);
        _skinPreviewRenderer = new SkinPreviewRenderer();
        _skinPreviewRenderer.SetUp();
    }
    
    private async void OnSkinFileDrop(object sender, DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

        string[] files = (string[])e.Data.GetData(DataFormats.FileDrop)!;
        if (files.Length != 1 || !files[0].EndsWith(".png")) return;

        FileInfo fileInfo = new FileInfo(files[0]);
        Skin skin = Skin.FromFile(fileInfo);
        if (!await _fileClient.TryChangeSkin(_user, skin)) return;
        
        Utils.SaveSkin(fileInfo.FullName);
        _skinPreviewRenderer.ChangeSkin(_user.SkinPath);
    }
    
    private void SkinPreviewOnRender(TimeSpan obj)
    {
        _skinPreviewRenderer.Render((float)SkinSlider.Value);
    }

    #endregion

    #region Navigation

    private void MoveWindow(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            if (WindowState == WindowState.Maximized) 
            {
                WindowState = WindowState.Normal;
                Application.Current.MainWindow!.Top = 3;
            }
            DragMove();
        }
    }

    private void ShutDown(object sender, RoutedEventArgs e)
    {
        try
        {
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

    #region Loading

    private sealed class LoadingScreen
    {
        private readonly Border _loading;
        private readonly RadialProgressBar _progressBar;
        private readonly TextBlock _progressText;
        private CompositeProgress _progress;

        public LoadingScreen(Border loading, RadialProgressBar progressBar, TextBlock progressText)
        {
            _loading = loading;
            _progressBar = progressBar;
            _progressText = progressText;
        }

        public CompositeProgress Show()
        {
            _loading.Visibility = Visibility.Visible;
            _progressBar.Value = 0f;
            _progressText.Text = "0";
            _progress = new CompositeProgress();
            _progress.Changed += OnProgressChange;
            return _progress;
        }

        public void Hide()
        {
            _loading.Visibility = Visibility.Collapsed;
            _progress.Changed -= OnProgressChange;
        }

        private void OnProgressChange(float progressValue)
        {
            _progressBar.Value = progressValue * 100f;
            _progressText.Text = $"{(int)(progressValue * 100f)}";
        }
    }

    #endregion
    
    private void ApplyLocalPrefs()
    {
        RamSlider.Value = _localPrefs.DedicatedRAM;
        NicknameText.Text = _user.Nickname;
        ChangeLog.Text = _configVersion.Changelog;
    }

    private void SaveLocalPrefs()
    {
        _localPrefs.IsFullscreen = FullScreen.IsChecked!.Value;
        _localPrefs.DedicatedRAM = (int)RamSlider.Value;
        LocalPrefs.SaveLocalPrefs(_localPrefs);
    }
    
    private void ExitAccount(object sender, RoutedEventArgs e)
    {
        LocalPrefs.Clear();
        LogInWindow logInWindow = new LogInWindow(_fileClient, _launchConfig, _configVersion);
        logInWindow.Show();
        Close();
    }

    private async void OnReloadFiles(object sender, RoutedEventArgs e)
    {
        CompositeProgress progress = _loadingScreen.Show();
        await _game.CleanInstall(progress);
        _loadingScreen.Hide();
    }

    private void OnOpenFolderButton(object sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo(new MinecraftPath().BasePath) { UseShellExecute = true });
    }
}