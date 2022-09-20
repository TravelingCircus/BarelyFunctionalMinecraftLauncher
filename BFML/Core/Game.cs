using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Version;
using CommonData;
using FileClient;

namespace BFML.Core;

public sealed class Game
{
    public readonly MVersion VanillaVersion;
    public readonly MVersion ForgeVersion;

    private readonly BFMLFileClient _fileClient;
    private readonly CMLauncher _launcher;
    private readonly MinecraftPath _minecraftPath;
    private readonly Vanilla _vanilla;
    private readonly Forge _forge;

    private Game(BFMLFileClient fileClient)
    {
        _fileClient = fileClient;
        _minecraftPath = new MinecraftPath();
        _launcher = new CMLauncher(_minecraftPath);
        
        ForgeVersion.InheritFrom(VanillaVersion);
        _vanilla = new Vanilla(_minecraftPath, VanillaVersion);
        _forge = new Forge(_minecraftPath, ForgeVersion, _fileClient);
    }

    public static async Task<Game> SetUp(BFMLFileClient fileClient)
    {
        LaunchConfiguration configuration = await fileClient.DownloadLaunchConfiguration();
        throw new NotImplementedException();
    }

    public void Launch()
    {
        System.Net.ServicePointManager.DefaultConnectionLimit = 256;
        Process process = _launcher.CreateProcess(ForgeVersion, new MLaunchOption()
        {
            MaximumRamMb = 8192, //TODO Get values from somewhere else that my own ass
            Session = MSession.GetOfflineSession("BFML_TEST")
        },false);
        process.Start();
    }
    
    public Task Install()
    {
        DeleteAllFiles();
        InstallMinecraft();
        Task[] tasks = {
            InstallForge(),
            InstallMods()
        };
        return Task.WhenAll(tasks);
    }

    public bool IsReadyToLaunch()
    {
        throw new NotImplementedException();
        return _forge.IsInstalled();
    }

    public Task DeleteAllFiles()
    {
        string minecraftDirectory = _minecraftPath.BasePath;
        return Task.Run(() =>
        {
            if (!Directory.Exists(minecraftDirectory)) return;
            Directory.Delete(minecraftDirectory);
        });
    }

    private Task InstallMinecraft()
    {
        return _launcher.CheckAndDownloadAsync(VanillaVersion);
    }

    private Task InstallForge()
    {
        return Task.CompletedTask;
    }
    
    private Task InstallMods()
    {
        throw new NotImplementedException();
    }
}