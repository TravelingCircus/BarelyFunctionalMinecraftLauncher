using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using BFML.Repository;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Version;
using Common.Misc;
using FileClient.Utils;

namespace BFML.Core;

internal sealed class Game
{
    private readonly Repo _repo;
    private readonly CMLauncher _launcher;

    internal Game(Repo repo)
    {
        _repo = repo;
        MinecraftPath minecraftPath = new MinecraftPath();
        _launcher = new CMLauncher(minecraftPath);
    }

    public Task Launch(string nickname, MVersion vanilla, bool isModded, Forge forge = null, ModPack modPack = null)
    {
        LaunchConfiguration launchConfiguration = new LaunchConfiguration()
        {
            Nickname = nickname,
            IsModded = isModded,
            VanillaVersion = vanilla,
            ForgeVersion = forge,
            ModPack = modPack,
            DedicatedRam = _repo.LocalPrefs.DedicatedRam,
            FullScreen = _repo.LocalPrefs.IsFullscreen,
            Validation = _repo.LocalPrefs.FileValidationMode
        };
        return StartGameProcess(launchConfiguration);
    }

    private async Task StartGameProcess(LaunchConfiguration launchConfiguration)
    {
        MVersion v = await _launcher.GetVersionAsync("1.18.2");
        
        await InstallVanilla(v, new ProgressTracker());
        
        System.Net.ServicePointManager.DefaultConnectionLimit = 256;
        Process process = await _launcher.CreateProcessAsync(v, new MLaunchOption
        {
            MaximumRamMb = launchConfiguration.DedicatedRam,
            Session = MSession.GetOfflineSession(launchConfiguration.Nickname),
            FullScreen = launchConfiguration.FullScreen
        }, false);
        process.Start();
    }
    
    public async Task CleanInstall(LaunchConfiguration launchConfiguration, CompositeProgress progress)
    {
        DeleteAllFiles(progress.AddTracker(0.1f));

        MVersion vanillaVersion = launchConfiguration.VanillaVersion;
        await InstallVanilla(vanillaVersion, progress.AddTracker(0.5f));

        await _launcher.GetAllVersionsAsync();
    }
    
    public bool IsReadyToLaunch()
    {
        return true; //TODO
    }

    public void DeleteAllFiles(ProgressTracker progress)
    {
        DirectoryInfo gameDirectory = _repo.LocalPrefs.GameDirectory;
        
        if (!gameDirectory.Exists) return;
        foreach (FileInfo file in gameDirectory.GetFiles())
        {
            file.Delete();
        }
        progress.Add(0.5f);
        foreach (DirectoryInfo directory in gameDirectory.GetDirectories())
        {
            if (directory.Name != "BFML" && directory.Name != "saves") directory.Delete(true);
        }
        progress.Add(0.5f);
    }

    private async Task InstallVanilla(MVersion version, ProgressTracker progressTracker)
    {
        Task download = _launcher.CheckAndDownloadAsync(version);
        
        while (!download.IsCompleted && !download.IsFaulted)
        {
            await Task.Delay(300);
            /*float downloaded = _repo.LocalPrefs.GameDirectory.RoughSize();
            float toDownload = 600 * 1024 * 1024f;
            progressTracker.Report(downloaded / toDownload);*/
        }
        await download; 
        progressTracker.Report(1f);
    }
}