using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Version;
using CommonData;
using CommonData.Models;
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

    private Game(FileClient fileClient, MinecraftPath minecraftPath, Vanilla vanilla, Forge forge, Mods mods)
    {
        _fileClient = fileClient;
        _minecraftPath = minecraftPath;
        Vanilla = vanilla;
        Forge = forge;
        Mods = mods;
        _launcher = new CMLauncher(_minecraftPath);
    }

    public static Task<Game> SetUp(FileClient fileClient, LaunchConfiguration configuration)
    {
        MinecraftPath path = new MinecraftPath();
        MVersion vanillaVersion = new MVersion(configuration.VanillaVersion);
        MVersion forgeVersion = new MVersion(configuration.ForgeVersion);
        forgeVersion.InheritFrom(vanillaVersion);
        
        return Task.FromResult(new Game(fileClient, path,
            new Vanilla(vanillaVersion, path),
            new Forge(forgeVersion, path),
            new Mods(path)));
    }

    public void Launch(int ram, bool fullScreen, string nickname)
    {
        System.Net.ServicePointManager.DefaultConnectionLimit = 256;
        Process process = _launcher.CreateProcess(Forge.Version, new MLaunchOption
        {
            MaximumRamMb = ram,
            Session = MSession.GetOfflineSession(nickname),
            FullScreen = fullScreen
        },false);
        process.Start();
    }
    
    public async Task CleanInstall()
    {
        DeleteAllFiles();
        await InstallMinecraft().ConfigureAwait(false);
        Task[] tasks = {
            InstallForge(),
            InstallMods()
        };
        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    public bool IsReadyToLaunch()
    {
        return
            Vanilla.IsInstalled()
            && Forge.IsInstalled() 
            && Mods.ChecksumMatches(_launchConfiguration.ModsChecksum);
    }

    public void DeleteAllFiles()
    {
        string minecraftDirectory = _minecraftPath.BasePath;
        if (!Directory.Exists(minecraftDirectory)) return;
        DirectoryInfo directoryInfo = new DirectoryInfo(minecraftDirectory);
        foreach (FileInfo file in directoryInfo.GetFiles())
        {
            file.Delete();
        }
        foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
        {
            if (directory.Name != "BFML") directory.Delete();
        }

    }

    private Task InstallMinecraft()
    {
        return _launcher.CheckAndDownloadAsync(Vanilla.Version);
    }

    private async Task InstallForge()
    {
        string forgeFilesZip = await _fileClient.DownloadForgeFiles();
        await Forge.Install(forgeFilesZip).ConfigureAwait(false);
    }
    
    private async Task InstallMods()
    {
        using (TempDirectory tempDirectory = new TempDirectory())
        {
            await _fileClient.DownloadMods(tempDirectory.Info.FullName);
            Mods.InstallFromFolder(tempDirectory.Info.FullName);
        }
    }
}