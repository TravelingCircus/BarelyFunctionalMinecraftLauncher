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
using CommonData.Network.Messages.Login;
using CommonData.Network.Messages.Registration;
using TCPFileClient;

namespace BFML.WPF;

public partial class MainWindow : Window
{
    private readonly SkinPreviewRenderer _skinPreviewRenderer;

    public MainWindow()
    {
        InitializeComponent();
        /*GLWpfControlSettings settings = new GLWpfControlSettings
        {
            MajorVersion = 4,
            MinorVersion = 0
        };*/
        //OpenTkControl.Start(settings);
        //_skinPreviewRenderer = new SkinPreviewRenderer();
        //_skinPreviewRenderer.SetUp();
        ChangeLog.Text = "";
    }

    public MainWindow(FileClient fileClient, User user, LocalPrefs localPrefs, 
        string skinPath, ConfigurationVersion configVersion, LaunchConfiguration launchConfig)
    {
        
    }

    private FileClient _fileClient;

    private async void PlayButtonOnClick(object sender, RoutedEventArgs e)
    {
        _fileClient = ConnectToServer();
        User user = new User("pisos", "iousdgfab");

        LogLine("SENT REGISTRATION REQUEST");
        RegistrationResponse registrationResponse = await Register(user);
        LogLine($"RESULT: {registrationResponse.Success}");

        LogLine("SENT LOGIN REQUEST");
        LoginResponse loginResponse = await Login(user);
        LogLine($"RESULT: {loginResponse.Success}");

        LogLine("SENT LC REQUEST");
        LaunchConfiguration launchConfiguration = await GetConfig();
        LogLine($"LAUNCH CONFIGURATION: [valilla:{launchConfiguration.VanillaVersion}] [forge:{launchConfiguration.ForgeVersion}]");
    }

    private FileClient ConnectToServer()
    {
        FileClient fileClient = new FileClient();
        string log = fileClient.ConnectToServer() ? "CONNECTED" : "FAILED TO CONNECT";
        //LogLine(log);
        return fileClient;
    }

    private Task<RegistrationResponse> Register(User user)
    {
        return _fileClient.SendRegistrationRequest(user);
    }

    private Task<LoginResponse> Login(User user)
    {
        return _fileClient.SendLoginRequest(user);
    }

    private Task<LaunchConfiguration> GetConfig()
    {
        return _fileClient.DownloadLaunchConfiguration();
    }

    private async void SkinPreviewOnRender(TimeSpan obj)
    {
        GL.ClearColor(Color4.White);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        //await _skinPreviewRenderer.Render(obj).ConfigureAwait(false);
    }

    #region Navigation

    private void MoveWindow(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            if (this.WindowState == WindowState.Maximized) 
            {
                this.WindowState = WindowState.Normal;
                Application.Current.MainWindow.Top = 3;
            }
            this.DragMove();
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

    private void LogLine(string line)
    {
        ChangeLog.Text += "\n" + line;
    }
    
    private async void SkinFileDrop_Drop(object sender, DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

        string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
        string pngDirectory = Path.GetDirectoryName(files[0]);
        string pngName = Path.GetFileName(files[0]);

        await _fileClient.SendSkinChangeRequest("pisos", pngDirectory, pngName);
    }
}