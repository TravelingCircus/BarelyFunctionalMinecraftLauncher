using System.Diagnostics;
using System.Threading.Tasks;
using BFML.Repository;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Version;

namespace BFML.Core;

internal sealed class Game
{
    internal MVersionCollection Versions => _launcher.Versions;
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
        LaunchConfiguration launchConfig = new LaunchConfiguration()
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
        return StartGameProcess(launchConfig);
    }

    private async Task StartGameProcess(LaunchConfiguration launchConfig)
    {
        if (launchConfig.Validation == FileValidation.Full) await _launcher.CheckAndDownloadAsync(launchConfig.VanillaVersion);
        
        System.Net.ServicePointManager.DefaultConnectionLimit = 256;
        Process process = await _launcher.CreateProcessAsync(launchConfig.VanillaVersion, new MLaunchOption
        {
            MaximumRamMb = launchConfig.DedicatedRam,
            Session = MSession.GetOfflineSession(launchConfig.Nickname),
            FullScreen = launchConfig.FullScreen
        }, false);
        process.Start();
    }
    
    public bool IsReadyToLaunch()
    {
        return true; //TODO
    }
}