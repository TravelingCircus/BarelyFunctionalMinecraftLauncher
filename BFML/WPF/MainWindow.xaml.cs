using System;
using System.Threading.Tasks;
using System.Windows;
using BFML._3D;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Windows.Input;
using CommonData.Models;
using CommonData.Network.Messages;
using TCPFileClient;

namespace BFML.WPF;

public partial class MainWindow : Window
{
    private readonly SkinPreviewRenderer _skinPreviewRenderer;

    public MainWindow()
    {
        InitializeComponent();
        
        ClearLogs();
        LogLine("YA YEBY SOBAK");
        
        /*GLWpfControlSettings settings = new GLWpfControlSettings
        {
            MajorVersion = 4,
            MinorVersion = 0
        };*/
        //OpenTkControl.Start(settings);
        //_skinPreviewRenderer = new SkinPreviewRenderer();
        //_skinPreviewRenderer.SetUp();
    }

    private FileClient _fileClient;

    private async void PlayButton(object sender, RoutedEventArgs e)
    {
        _fileClient = ConnectToServer();
        User user = new User("pisos", "iousdgfab");
        
        LogLine("SENT REGISTRATION REQUEST");
        RegistrationResponse registrationResponse = await Register(user);
        LogLine($"RESULT: {registrationResponse.Success}");
        
        LogLine("SENT LOGIN REQUEST");
        LoginResponse loginResponse = await Login(user);
        LogLine($"RESULT: {loginResponse.Success}");

    }
    
    private FileClient ConnectToServer()
    {
        FileClient fileClient = new FileClient();
        string log = fileClient.ConnectToServer() ? "CONNECTED" : "FAILED TO CONNECT";
        LogLine(log);
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
    
    private async void SkinPreviewOnRender(TimeSpan obj)
    {
        GL.ClearColor(Color4.White);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        //await _skinPreviewRenderer.Render(obj).ConfigureAwait(false);
    }

    #region Navigation
    
    private void MoveWindow(object sender, MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed) return;
        
        if (WindowState == WindowState.Maximized) 
        {
            WindowState = WindowState.Normal;
            Application.Current.MainWindow.Top = 3;
        }
        DragMove();
    }

    private void ShutDown(object sender, RoutedEventArgs e)
    {
        App.Current.Shutdown();
    }
    
    private void Minimize(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    #endregion

    #region Logs

    private void LogLine(string line)
    {
        ChangeLog.Text += "\n" + line;
    }

    private void ClearLogs()
    {
        ChangeLog.Text = String.Empty;
    }

    #endregion
}