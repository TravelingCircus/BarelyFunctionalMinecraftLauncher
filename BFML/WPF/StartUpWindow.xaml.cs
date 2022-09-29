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
        
        ConfigurationVersion version = await fileClient.DownloadConfigVersion();
        LaunchConfiguration launchConfig = await fileClient.DownloadLaunchConfiguration();

        LoginResponse loginResponse = await TryLogIn(fileClient, localPrefs);
        if (loginResponse.Success)
        {
            MainWindow mainWindow = new MainWindow(
                fileClient, 
                loginResponse.User, 
                localPrefs,
                launchConfig, 
                version);
            
            mainWindow.Show();
            Close();
        }
        else
        {
            LogInWindow logInWindow = new LogInWindow();
            logInWindow.Show();
            Close();
        }
    }
    
    private FileClient ConnectToServer()
    {
        FileClient fileClient = new FileClient();
        bool success = fileClient.ConnectToServer();
        if (!success) throw new Exception("Failed connect to the server");
        return fileClient;
    }

    private Task<LoginResponse> TryLogIn(FileClient fileClient, LocalPrefs localPrefs)
    {
        User user = new User(localPrefs.Nickname, localPrefs.Password);
        return fileClient.SendLoginRequest(user);
    }
}