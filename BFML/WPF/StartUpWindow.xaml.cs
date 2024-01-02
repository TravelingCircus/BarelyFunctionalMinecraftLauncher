using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using BFML.Core;
using BFML.Repository;
using ICSharpCode.SharpZipLib;
using Utils.Async;

namespace BFML.WPF;

public partial class StartUpWindow
{
    public StartUpWindow()
    {
        InitializeComponent();
        Loaded += OnWindowLoaded;
    }

    private void OnWindowLoaded(object obj, RoutedEventArgs args)
    {
        Loaded -= OnWindowLoaded;
        Init().FireAndForget();
    }

    private async Task Init()
    {
        FontInstaller.InstallFont(new FileInfo(Environment.CurrentDirectory + "\\Repo\\Resources\\MinecraftFont.ttf"));

        RepoIO repoIo = new RepoIO(new DirectoryInfo(Directory.GetCurrentDirectory()+"\\Repo"));
        repoIo.Validate().Match(
            _ => { }, 
            err => throw err);

        LauncherMode startLauncherMode = (await repoIo.Configs.LoadLocalPrefs()).LauncherMode;

        if (startLauncherMode == LauncherMode.Manual) await StartManualMode(repoIo);
        else if (startLauncherMode == LauncherMode.Centralized) StartCentralizedMode(repoIo);
        else throw new ValueOutOfRangeException(nameof(startLauncherMode));
        
        Close();
    }
    
    private async Task StartManualMode(RepoIO repoIo)
    {
        ManualModeRepo repo = new ManualModeRepo(repoIo);
        await repo.TryInit();
        ManualModeWindow mainWindow = new ManualModeWindow(repo)
        {
            Top = Top,
            Left = Left + Width / 2
        };
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