using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using BFML._3D;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Wpf;
using System.Windows.Input;
using BFML.Core;
using CommonData.Models;
using TCPFileClient;

namespace BFML.WPF;

public partial class MainWindow : Window
{
    private readonly FileClient _fileClient;
    private readonly User _user;
    private readonly LocalPrefs _localPrefs;
    private readonly string _skinPath;
    private readonly ConfigurationVersion _configVersion;
    private readonly LaunchConfiguration _launchConfig;
    private readonly SkinPreviewRenderer _skinPreviewRenderer;

    public MainWindow(FileClient fileClient, User user, LocalPrefs localPrefs, string skinPath, LaunchConfiguration launchConfig, ConfigurationVersion configVersion)
    {
        InitializeComponent();
        _fileClient = fileClient;
        _user = user;
        _localPrefs = localPrefs;
        _skinPath = skinPath;
        _launchConfig = launchConfig;
        _configVersion = configVersion;

        Loaded += OnWindowLoaded;
    }

    private async void OnWindowLoaded(object sender, RoutedEventArgs args)
    {
        
    }
    
    #region PlayerModelRendering

    private void SetUpSkinRenderer()
    {
        GLWpfControlSettings settings = new GLWpfControlSettings
        {
            MajorVersion = 4,
            MinorVersion = 0
        };
        // OpenTkControl.Start(settings);
        // _skinPreviewRenderer = new SkinPreviewRenderer();
        // _skinPreviewRenderer.SetUp();
    }
    
    private async void SkinPreviewOnRender(TimeSpan obj)
    {
        GL.ClearColor(Color4.White);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        //await _skinPreviewRenderer.Render(obj).ConfigureAwait(false);
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
                Application.Current.MainWindow.Top = 3;
            }
            DragMove();
        }
    }

    private void ShutDown(object sender, RoutedEventArgs e)
    {
        try
        {
            App.Current.Shutdown();
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
            this.WindowState = WindowState.Minimized;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    #endregion

    private async void OnSkinFileDrop(object sender, DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

        string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
        string pngDirectory = Path.GetDirectoryName(files[0]);
        string pngName = Path.GetFileName(files[0]);

        await _fileClient.SendSkinChangeRequest("pisos", pngDirectory, pngName);
    }
}