using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Version;
using CommonData;
using TCPFileClient;

namespace BFML.Core;

public sealed class Game
{
    public readonly Vanilla Vanilla;
    public readonly Forge Forge;
    public readonly Mods Mods;
    private readonly FileClient _fileClient;
    private readonly CMLauncher _launcher;
    private readonly MinecraftPath _minecraftPath;

    private Game(FileClient fileClient, MinecraftPath minecraftPath, Vanilla vanilla, Forge forge, Mods mods)
    {
        _fileClient = fileClient;
        _minecraftPath = minecraftPath;
        Vanilla = vanilla;
        Forge = forge;
        Mods = mods;
        _launcher = new CMLauncher(_minecraftPath);
    }

    public static async Task<Game> SetUp(FileClient fileClient)
    {
        LaunchConfiguration configuration = await fileClient.DownloadLaunchConfiguration();

        MinecraftPath path = new MinecraftPath();
        MVersion vanillaVersion = new MVersion(configuration.VanillaVersion);
        MVersion forgeVersion = new MVersion(configuration.VanillaVersion+"-forge-"+configuration.ForgeVersion);
        forgeVersion.InheritFrom(vanillaVersion);
        
        return new Game(fileClient, path,
            new Vanilla(vanillaVersion, path),
            new Forge(forgeVersion, path),
            new Mods(path));
    }

    public void Launch()
    {
        System.Net.ServicePointManager.DefaultConnectionLimit = 256;
        Process process = _launcher.CreateProcess(Forge.Version, new MLaunchOption
        {
            MaximumRamMb = 8192, //TODO Get values from somewhere other than my own ass
            Session = MSession.GetOfflineSession("BFML_TEST")
        },false);
        process.Start();
    }
    
    public async Task Install()
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
        throw new NotImplementedException();
        return Forge.IsInstalled();
    }

    public void DeleteAllFiles()
    {
        string minecraftDirectory = _minecraftPath.BasePath;
        if (!Directory.Exists(minecraftDirectory)) return;
        
        Directory.Delete(minecraftDirectory);
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
    
    private Task InstallMods()
    {
        throw new NotImplementedException();
    }
}