using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using BFML.Core;
using BFML.Repository;
using ICSharpCode.SharpZipLib;

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
        try
        {
            await Init();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task Init()
    {
        FontInstaller.InstallFont(new FileInfo(Environment.CurrentDirectory + "\\MinecraftFont.ttf"));

        RepoIO repoIo = new RepoIO(new DirectoryInfo(""));
        repoIo.Validate().Match(
            _ => { }, 
            err => throw err);

        LauncherMode startLauncherMode = (await repoIo.Configs.LoadLocalPrefs()).LastLauncherMode;

        if (startLauncherMode == LauncherMode.Manual) StartManualMode(repoIo);
        else if (startLauncherMode == LauncherMode.Centralized) StartCentralizedMode(repoIo);
        else throw new ValueOutOfRangeException(nameof(startLauncherMode));
        
        Close();
    }
    
    private void StartManualMode(RepoIO repoIo)
    {
        MainWindow mainWindow = new MainWindow(new ManualModeRepo(repoIo));
        mainWindow.Top = Top;
        mainWindow.Left = Left + Width / 2;
        mainWindow.Show();
    }

    private void StartCentralizedMode(RepoIO repoIo)
    {
        /*LogInWindow logInWindow = new LogInWindow(new CentralizedModeRepo(repoIo));
        logInWindow.Top = (Top + Height / 2) / 2;
        logInWindow.Left = Left + logInWindow.Width / 2;
        logInWindow.Show();*/
    }
}