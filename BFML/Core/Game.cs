using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Version;
using Common;
using Common.Misc;
using Common.Models;
using FileClient.Utils;

namespace BFML.Core;

public sealed class Game
{
    public readonly Mods Mods;
    public readonly Forge Forge;
    public readonly Vanilla Vanilla;
    private readonly IFileClient _fileClient;
    private readonly CMLauncher _launcher;
    private readonly MinecraftPath _minecraftPath;
    private readonly LaunchConfiguration _launchConfiguration;

    private Game(IFileClient fileClient, LaunchConfiguration launchConfiguration, CMLauncher launcher, Vanilla vanilla,
        Forge forge, MinecraftPath path)
    {
        _minecraftPath = path;
        _launchConfiguration = launchConfiguration;
        _fileClient = fileClient;
        Vanilla = vanilla;
        Forge = forge;
        Mods = new Mods(_minecraftPath);
        _launcher = launcher;
    }

    public static async Task<Game> SetUp(IFileClient fileClient, LaunchConfiguration launchConfiguration)
    {
        MinecraftPath minecraftPath = new MinecraftPath();
        CMLauncher launcher = new CMLauncher(minecraftPath);
        MVersion vanillaVersion = await launcher.GetVersionAsync(launchConfiguration.VanillaVersion);
        MVersion forgeVersion = new MVersion(launchConfiguration.ForgeVersion);
        forgeVersion.InheritFrom(vanillaVersion);
        Vanilla vanilla = new Vanilla(vanillaVersion, minecraftPath);
        Forge forge = new Forge(forgeVersion, minecraftPath);

        return new Game(fileClient, launchConfiguration, launcher, vanilla, forge, minecraftPath);
    }

    public async Task Launch(int ram, bool fullScreen, string nickname)
    {
        System.Net.ServicePointManager.DefaultConnectionLimit = 256;
        Process process = await _launcher.CreateProcessAsync(Forge.Version.Id, new MLaunchOption
        {
            MaximumRamMb = ram,
            Session = MSession.GetOfflineSession(nickname),
            FullScreen = fullScreen
        }, true);
        process.Start();
    }

    public async Task CleanInstall(CompositeProgress progress)
    {
        DeleteAllFiles(progress.AddTracker(0.1f));
        
        await InstallVanilla(progress.AddTracker(0.5f));
        
        if (!await TryInstallForge(progress.AddTracker(0.1f))) 
            throw new InvalidOperationException("Can't install forge."); //TODO Refactor
        
        if (!await TryInstallMods(progress.AddTracker(0.3f))) 
            throw new InvalidOperationException("Can't install mods."); //TODO Refactor
        
        await _launcher.GetAllVersionsAsync();
    }
    
    public bool IsReadyToLaunch()
    {
        return
            Vanilla.IsInstalled()
            && Forge.IsInstalled(_launchConfiguration)
            && Mods.ChecksumMatches(uint.Parse(_launchConfiguration.ModsChecksum));
    }

    public void DeleteAllFiles(ProgressTracker progress)
    {
        string minecraftDirectory = _minecraftPath.BasePath;
        if (!Directory.Exists(minecraftDirectory)) return;
        DirectoryInfo directoryInfo = new DirectoryInfo(minecraftDirectory);
        foreach (FileInfo file in directoryInfo.GetFiles())
        {
            file.Delete();
        }
        progress.Add(0.5f);
        foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
        {
            if (directory.Name != "BFML" && directory.Name != "saves") directory.Delete(true);
        }
        progress.Add(0.5f);
    }

    private async Task InstallVanilla(ProgressTracker progressTracker)
    {
        Task download = _launcher.CheckAndDownloadAsync(Vanilla.Version);
        DirectoryInfo minecraftDirectory = new DirectoryInfo(_minecraftPath.BasePath);
        
        while (!download.IsCompleted && !download.IsFaulted)
        {
            await Task.Delay(300);
            float downloaded = minecraftDirectory.RoughSize();
            float toDownload = 600 * 1024 * 1024f;
            progressTracker.Report(downloaded / toDownload);
        }
        await download; 
        progressTracker.Report(1f);
    }
    
    private async Task<bool> TryInstallForge(ProgressTracker progressTracker)
    {
        using TempDirectory tempDirectory = new TempDirectory();
        FileInfo targetFile = new FileInfo(tempDirectory.Info.FullName + "Forge.zip");
        
        if (!await _fileClient.TryLoadForge(targetFile)) return false;
        
        progressTracker.Add(0.8f);
        await Forge.Install(targetFile.FullName, _launchConfiguration);
        progressTracker.Add(0.2f);
        return true;
    }

    private async Task<bool> TryInstallMods(ProgressTracker progressTracker)
    {
        if (!Directory.Exists(_minecraftPath.BasePath + @"\mods"))
            Directory.CreateDirectory(_minecraftPath.BasePath + @"\mods");
        using TempDirectory tempDirectory = new TempDirectory();
        FileInfo targetFile = new FileInfo(tempDirectory.Info.FullName + "Mods.zip");
        
        if (!await _fileClient.TryLoadMods(targetFile)) return false;
        
        progressTracker.Add(0.8f);
        await Mods.InstallFromArchive(targetFile.FullName);
        progressTracker.Add(0.2f);
        return true;
    }
}