using System;
using System.Threading.Tasks;
using System.Windows;
using BFML.Core;
using BFML.WPF;
using CommonData.Models;
using CommonData.Network.Messages.Login;
using TCPFileClient;

namespace BFML;

public partial class StartUpWindow : Window
{
    public StartUpWindow()
    {
        InitializeComponent();

        Loaded += OnWindowLoaded;
    }

    private async void OnWindowLoaded(object obj, RoutedEventArgs args)
    {
        FileClient fileClient = ConnectToServer();
        LocalPrefs localPrefs = LocalPrefs.GetLocalPrefs();
        
        ConfigurationVersion version = await GetVersion(fileClient, localPrefs);
        LaunchConfiguration launchConfig = await GetLaunchConfig(fileClient, localPrefs);
        
        if (await TryLogIn(fileClient, localPrefs))
        {
            User user = await GetUser(fileClient, localPrefs);
            //TODO Download userSkin from server, save on PC and rewrite skinPath;
            MainWindow mainWindow = new MainWindow(fileClient, user, localPrefs, user.SkinPath, launchConfig, version);
            mainWindow.Show();
        }
        else
        {
            //TODO Open LogIn window
        }
    }
    
    private FileClient ConnectToServer()
    {
        FileClient fileClient = new FileClient();
        bool success = fileClient.ConnectToServer();
        if (!success) throw new Exception("Failed connect to the server");
        return fileClient;
    }

    private async Task<bool> TryLogIn(FileClient fileClient, LocalPrefs localPrefs)
    {
        User user = new User(localPrefs.Nickname, localPrefs.Password);
        //TODO rewrite LogIn response(Get user from it)
        LoginResponse response = await fileClient.SendLoginRequest(user);
        return response.Success;
    }

    private async Task<User> GetUser(FileClient fileClient, LocalPrefs localPrefs)
    {
        //TODO rewrite LogIn response
        throw new NotImplementedException();
    }

    private async Task<ConfigurationVersion> GetVersion(FileClient fileClient, LocalPrefs localPrefs)
    {
        return await fileClient.DownloadConfigVersion();
    }
    
    private async Task<LaunchConfiguration> GetLaunchConfig(FileClient fileClient, LocalPrefs localPrefs)
    {
        return await fileClient.DownloadLaunchConfiguration();
    }
}