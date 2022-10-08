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

    public Game(FileClient fileClient, LaunchConfiguration launchConfiguration)
    {
        _launchConfiguration = launchConfiguration;
        _fileClient = fileClient;
        _minecraftPath = new MinecraftPath();
        Vanilla = new Vanilla(new MVersion(launchConfiguration.VanillaVersion), _minecraftPath);
        Forge = new Forge(new MVersion(launchConfiguration.ForgeVersion), _minecraftPath);
        Mods = new Mods(_minecraftPath);
        _launcher = new CMLauncher(_minecraftPath);
    }

    public void Launch(int ram, bool fullScreen, string nickname)
    {
        System.Net.ServicePointManager.DefaultConnectionLimit = 256;
        Process process = _launcher.CreateProcess(Vanilla.Version, new MLaunchOption
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