using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Version;
using Common.Models;
using Common.Network.Messages.ForgeDownload;
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

    public void Launch(int ram, bool fullScreen, string nickname)
    {
        System.Net.ServicePointManager.DefaultConnectionLimit = 256;
        Process process = _launcher.CreateProcess(Vanilla.Version, new MLaunchOption
        {
            MaximumRamMb = ram,
            Session = MSession.GetOfflineSession(nickname),
            FullScreen = fullScreen
        }, false);
        process.Start();
    }

    public async Task CleanInstall(IProgress<double> progress)
    {
        DeleteAllFiles();
        await _launcher.CheckAndDownloadAsync(Forge.Version);
        Task[] tasks =
        {
            InstallForge(),
            InstallMods()
        };
        await Task.WhenAll(tasks);
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
            if (directory.Name != "BFML") directory.Delete(true);
        }
    }

    private async Task InstallForge()
    {
        using (TempDirectory tempDirectory = new TempDirectory())
        {
            ForgeDownloadResponse response = await _fileClient.DownloadForgeFiles(tempDirectory.Info.FullName);
            await Forge.Install(response.TempForgePath);
        }
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