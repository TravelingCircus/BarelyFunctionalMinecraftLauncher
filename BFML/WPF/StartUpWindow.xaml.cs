using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using BFML.Core;
using CmlLib.Core;
using Common;
using Common.Misc;
using Common.Models;
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

        Result<User> loginResult = await TryLogIn(fileClient, localPrefs);
        if (loginResult.IsOk)
        {
            MainWindow mainWindow = new MainWindow(
                fileClient, 
                loginResult.Value, 
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
        ServerConnection serverConnection = new ServerConnection("3.123.51.46", 69);
        
        bool success = false;
        for (int i = 0; i < 3; i++)
        {
            success = await serverConnection.TryInit();
            if(success) break;
            await Task.Delay(2500);
        }
        
        return success ? serverConnection : null;
    }

    private static Task<Result<User>> TryLogIn(IFileClient fileClient, LocalPrefs localPrefs)
    {
        User user = new User(localPrefs.Nickname, localPrefs.Password);
        return fileClient.Authenticate(user);
    }
}