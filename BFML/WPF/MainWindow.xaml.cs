using System;
using System.IO;
using System.Windows;
using BFML._3D;
using OpenTK.Wpf;
using System.Windows.Input;
using BFML.Core;
using CommonData.Models;
using CommonData.Network.Messages.Skin;
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

    private void OnWindowLoaded(object sender, RoutedEventArgs args)
    {
        CheckIfUserPaid();
        ApplyLocalPrefs();
        Loaded -= OnWindowLoaded;
    }

    private void OnPlayButton(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void DisablePlayButton()
    {
        throw new NotImplementedException();
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
            Utils.SaveSkin(_user.SkinPath);
            _skinPreviewRenderer.ChangeSkin(files[0]);
        }
    }
    
    private void SkinPreviewOnRender(TimeSpan obj)
    {
        _skinPreviewRenderer.Render();
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
    }

    private void CheckIfUserPaid()
    {
        if (_user.GryvnyasPaid < _launchConfig.RequiredGriwnas)
        {
            //DisablePlayButton();
        }
    }
    
    private void ExitAccount(object sender, RoutedEventArgs e)
    {
        LocalPrefs.Clear();
        LogInWindow logInWindow = new LogInWindow(_fileClient, _launchConfig, _configVersion);
        logInWindow.Show();
        Close();
    }
}