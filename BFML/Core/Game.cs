using System;
using System.IO;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Version;

namespace BFML.Core;

public class Game
{
    public readonly MVersion VanillaVersion = new MVersion("1.16.5");//TODO read from config-file/server
    public readonly MVersion ForgeVersion;//TODO read from config-file/server
    
    private readonly CMLauncher _launcher;
    private readonly MinecraftPath _minecraftPath;
    private readonly Vanilla _vanilla;
    private readonly Forge _forge;

    public Game()
    {
        _minecraftPath = new MinecraftPath();
        _launcher = new CMLauncher(_minecraftPath);
        _vanilla = new Vanilla(_minecraftPath, VanillaVersion);
        _forge = new Forge(_minecraftPath, VanillaVersion);
    }

    public void Launch()
    {
        _launcher.Launch(ForgeVersion.Id, new MLaunchOption()
        {
            MaximumRamMb = 6000, //TODO Get values from somewhere else that my own ass
            Session = MSession.GetOfflineSession("BFML_TEST")
        });
    }
    
    public async Task Install()
    {
        await DeleteAllFiles().ConfigureAwait(false);
        await InstallMinecraft().ConfigureAwait(false);
        await InstallForge().ConfigureAwait(false);
        await InstallMods().ConfigureAwait(false);
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