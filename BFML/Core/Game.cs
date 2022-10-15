using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Version;
using Common.Misc;
using Common.Models;
using Common.Network.Messages.ForgeDownload;
using Common.Network.Messages.ModsDownload;
using TCPFileClient;
using TCPFileClient.Utils;

namespace BFML.Core;

public sealed class Game
{
    public readonly Vanilla Vanilla;
    public readonly Forge Forge;
    public readonly Mods Mods;
    private readonly FileClient _fileClient;
    private readonly CMLauncher _launcher;
    private readonly MinecraftPath _minecraftPath;
    private readonly LaunchConfiguration _launchConfiguration;

    private Game(FileClient fileClient, LaunchConfiguration launchConfiguration, CMLauncher launcher, Vanilla vanilla,
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

    public static async Task<Game> SetUp(FileClient fileClient, LaunchConfiguration launchConfiguration)
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
        await Task.Delay(100);
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
        Task[] tasks =
        {
            InstallForge(progress.AddTracker(0.1f)),
            InstallMods(progress.AddTracker(0.3f))
        };
        await Task.WhenAll(tasks);
        await _launcher.GetAllVersionsAsync();
    }
    
    public bool IsReadyToLaunch()
    {
        return
            Vanilla.IsInstalled()
            && Forge.IsInstalled(_launchConfiguration)
            && Mods.ChecksumMatches(_launchConfiguration.ModsChecksum);
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
            if (directory.Name != "BFML") directory.Delete(true);
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
    
    private async Task InstallForge(ProgressTracker progressTracker)
    {
        using (TempDirectory tempDirectory = new TempDirectory())
        {
            ForgeDownloadResponse response = await _fileClient.DownloadForgeFiles(tempDirectory.Info.FullName);
            progressTracker.Add(0.8f);
            await Forge.Install(response.TempForgePath, _launchConfiguration);
            progressTracker.Add(0.2f);
        }
    }

    private async Task InstallMods(ProgressTracker progressTracker)
    {
        if (!Directory.Exists(_minecraftPath.BasePath + @"\mods"))
            Directory.CreateDirectory(_minecraftPath.BasePath + @"\mods");
        using (TempDirectory tempDirectory = new TempDirectory())
        {
            ModsDownloadResponse response = await _fileClient.DownloadMods(tempDirectory.Info.FullName);
            progressTracker.Add(0.8f);
            await Mods.InstallFromArchive(response.ModsZipPath);
            progressTracker.Add(0.2f);
        }
    }
}