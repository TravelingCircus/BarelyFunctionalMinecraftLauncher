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
        
        IFileClient fileClient = await ResolveFileClient();
        LocalPrefs localPrefs = LocalPrefs.GetLocalPrefs();
        
        ConfigurationVersion version = await fileClient.LoadConfigVersion();
        LaunchConfiguration launchConfig = await fileClient.LoadLaunchConfiguration();

        LoginResponse loginResponse = await TryLogIn(fileClient, localPrefs);
        if (loginResponse.Success)
        {
            MainWindow mainWindow = new MainWindow(
                fileClient, 
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
            LogInWindow logInWindow = new LogInWindow(fileClient, launchConfig, version);
            logInWindow.Top = (Top + Height / 2) / 2;
            logInWindow.Left = Left + logInWindow.Width / 2;
            logInWindow.Show();
            Close();
        }
    }

    private static async Task<IFileClient> ResolveFileClient()
    {
        Result<ServerConnection> serverConnection = await ConnectToServer();

        if (serverConnection.IsOk) return serverConnection.Value;

        return GetLocalFileServer().Value;
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