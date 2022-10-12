using System;
using System.IO;
using System.Windows;
using BFML._3D;
using OpenTK.Wpf;
using System.Windows.Input;
using BFML.Core;
using CmlLib.Core;
using CmlLib.Core.Auth;
using Common.Models;
using Common.Network.Messages.Skin;
using TCPFileClient;

namespace BFML.WPF;

public partial class MainWindow : Window
{
    private readonly FileClient _fileClient;
    private readonly User _user;
    private readonly LocalPrefs _localPrefs;
    private readonly ConfigurationVersion _configVersion;
    private readonly LaunchConfiguration _launchConfig;
    private SkinPreviewRenderer _skinPreviewRenderer;
    private Game _game;

    public MainWindow(FileClient fileClient, User user, LocalPrefs localPrefs,
       LaunchConfiguration launchConfig, ConfigurationVersion configVersion)
    {
        InitializeComponent();
        _fileClient = fileClient;
        _user = user;
        _localPrefs = localPrefs;
        //TODO should correspond to skin.png in .minecraft/BFML/skin.png
        _launchConfig = launchConfig;
        _configVersion = configVersion;

        SetUpSkinRenderer();
        Loaded += OnWindowLoaded;
    }

    private async void OnWindowLoaded(object sender, RoutedEventArgs args)
    {
        Loaded -= OnWindowLoaded;
        CheckIfUserPaid();
        ApplyLocalPrefs();
        _skinPreviewRenderer.ChangeSkin(_user.SkinPath);
        _game = await Game.SetUp(_fileClient, _launchConfig);
    }

    private async void OnPlayButton(object sender, RoutedEventArgs e)
    {
        if(_game is null) return;
        PlayButton.IsEnabled = false;
        
        if (!_game.IsReadyToLaunch()) await _game.CleanInstall(null);
        _game.Launch((int)RamSlider.Value, false, _user.Nickname);
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
        if (files.Length != 1) return;

        SkinChangeResponse response = await _fileClient.SendSkinChangeRequest(_user.Nickname, files[0]);
        if (response.Success)
        {
            Utils.SaveSkin(files[0]);
            _skinPreviewRenderer.ChangeSkin(_user.SkinPath);
        }
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

    private void ApplyLocalPrefs()
    {
        RamSlider.Value = _localPrefs.DedicatedRAM;
        NicknameText.Text = _user.Nickname;
        ChangeLog.Text = _configVersion.Changelog;
    }

    private void CheckIfUserPaid()
    {
        if (_user.GryvnyasPaid < _launchConfig.RequiredGriwnas)
        {
            DisablePlayButton();
        }
    }
    
    private void DisablePlayButton()
    {
        PlayButton.IsEnabled = false;
        PlayButton.Content = "Not Paid";
        PlayButton.Opacity = 0.5f;
    }
    
    private void ExitAccount(object sender, RoutedEventArgs e)
    {
        LocalPrefs.Clear();
        LogInWindow logInWindow = new LogInWindow(_fileClient, _launchConfig, _configVersion);
        logInWindow.Show();
        Close();
    }
}