﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using BFML._3D;
using OpenTK.Wpf;
using System.Windows.Input;
using BFML.Core;
using BFML.Repository;
using Common;

namespace BFML.WPF;

public partial class MainWindow
{
    private readonly ManualModeRepo _repo;
    private Game _game;
    private SkinPreviewRenderer _skinPreviewRenderer;
    private readonly LoadingScreen _loadingScreen;

    internal MainWindow(ManualModeRepo repo)
    {
        _repo = repo;
        
        InitializeComponent();
        _loadingScreen = new LoadingScreen(Loading, ProgressBar, ProgressText);

        SetUpSkinRenderer();
        Loaded += OnWindowLoaded;
    }

    private async void OnWindowLoaded(object sender, RoutedEventArgs args)
    {
        Loaded -= OnWindowLoaded;
        ApplyLocalPrefs();
        _skinPreviewRenderer.ChangeSkin(_repo.DefaultSkin);
    }

    private async void OnPlayButton(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(new NotImplementedException().Message, "Not quite there yet");
        
        /*if(_game is null) return;
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
        Close();*/
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
        _skinPreviewRenderer.SetUp(_repo.DefaultSkin.SkinBytes.ToArray(), 
            _repo.DefaultSkin.SkinBytes.ToArray());
    }
    
    private async void OnSkinFileDrop(object sender, DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
 
        string[] files = (string[])e.Data.GetData(DataFormats.FileDrop)!;
        if (files.Length != 1 || !files[0].EndsWith(".png")) return;

        FileInfo fileInfo = new FileInfo(files[0]);
        Skin skin = Skin.FromFile(fileInfo);
        //TODO rewrite saved skin
        _skinPreviewRenderer.ChangeSkin(skin);
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
        RamSlider.Value = _repo.LocalPrefs.DedicatedRAM;
        NicknameText.Text = _repo.LocalPrefs.Nickname;
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
}