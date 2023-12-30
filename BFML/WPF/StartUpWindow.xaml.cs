using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using BFML.Core;
using CmlLib.Core;
using Common;
using Common.Misc;
using Common.Models;
using Common.Network.Messages.Login;
using FileClient;

namespace BFML.WPF;

public partial class StartUpWindow
{
    public StartUpWindow()
    {
        InitializeComponent();
        Loaded += OnWindowLoaded;
    }

    private async void OnWindowLoaded(object obj, RoutedEventArgs args)
    {
        FontInstaller.InstallFont(new FileInfo(Environment.CurrentDirectory + "\\MinecraftFont.ttf"));
        
        IFileClient serverConnection = await ConnectToServer() switch
        {
            {IsOk:true} result => result.Value,
            _ => GetLocalFileServer().Value
        };
        LocalPrefs localPrefs = LocalPrefs.GetLocalPrefs();
        
        ConfigurationVersion version = await serverConnection.DownloadConfigVersion();
        LaunchConfiguration launchConfig = await serverConnection.DownloadLaunchConfiguration();

        LoginResponse loginResponse = await TryLogIn(serverConnection, localPrefs);
        if (loginResponse.Success)
        {
            MainWindow mainWindow = new MainWindow(
                serverConnection, 
                loginResponse.User, 
                localPrefs,
                launchConfig, 
                version);

            mainWindow.Top = Top;
            mainWindow.Left = Left + Width / 2;
            mainWindow.Show();
            Close();
        }
        else
        {
            LogInWindow logInWindow = new LogInWindow(serverConnection, launchConfig, version);
            logInWindow.Top = (Top + Height / 2) / 2;
            logInWindow.Left = Left + logInWindow.Width / 2;
            logInWindow.Show();
            Close();
        }
    }
    
    private static Result<IFileClient> GetLocalFileServer()
    {
        return new ServerConnection(new MinecraftPath().BasePath);
    }
    
    private static async Task<Result<ServerConnection>> ConnectToServer()
    {
        ServerConnection serverConnection = new ServerConnection(new MinecraftPath().BasePath);
        
        bool success = false;
        for (int i = 0; i < 3; i++)
        {
            success = serverConnection.ConnectToServer();
            if(success) break;
            await Task.Delay(2500);
        }
        
        return success ? serverConnection : null;
    }

    private static Task<LoginResponse> TryLogIn(IFileClient fileClient, LocalPrefs localPrefs)
    {
        User user = new User(localPrefs.Nickname, localPrefs.Password);
        return fileClient.SendLoginRequest(user);
    }
}