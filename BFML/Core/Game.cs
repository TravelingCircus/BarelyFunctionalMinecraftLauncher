using System.Diagnostics;
using System.Threading.Tasks;
using BFML.Repository;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;
using Version = Utils.Version;

namespace BFML.Core;

internal sealed class Game
{
    internal IVersionLoader Versions => _launcher.VersionLoader;
    private readonly Repo _repo;
    private readonly CMLauncher _launcher;

    internal Game(Repo repo)
    {
        _repo = repo;
        MinecraftPath minecraftPath = new MinecraftPath(_repo.LocalPrefs.GameDirectory.FullName);
        _launcher = new CMLauncher(minecraftPath);
    }

    public bool IsReadyToLaunch()
    {
        return true; //TODO
    }

    public async Task Launch(string nickname, Version vanilla, bool isModded, Forge forge, ModPack modPack)
    {
        MVersion version = isModded
            ? await _launcher.GetVersionAsync(forge.Name)
            : await _launcher.GetVersionAsync(vanilla.ToString());
        LaunchConfiguration launchConfig = new LaunchConfiguration()
        {
            Nickname = nickname,
            IsModded = isModded,
            VanillaVersion = version,
            ForgeVersion = forge,
            ModPack = modPack,
            DedicatedRam = _repo.LocalPrefs.DedicatedRam,
            FullScreen = _repo.LocalPrefs.IsFullscreen,
            Validation = _repo.LocalPrefs.FileValidationMode
        };
        await StartGameProcess(launchConfig);
    }

    private async Task StartGameProcess(LaunchConfiguration launchConfig)
    {
        if (launchConfig.Validation == FileValidation.Full) await _launcher.CheckAndDownloadAsync(launchConfig.VanillaVersion);
        
        System.Net.ServicePointManager.DefaultConnectionLimit = 256;
        Process process = await _launcher.CreateProcessAsync(launchConfig.VanillaVersion, new MLaunchOption
        {
            MaximumRamMb = launchConfig.DedicatedRam,
            Session = MSession.CreateOfflineSession(launchConfig.Nickname),
            FullScreen = launchConfig.FullScreen
        }, false);
        process.Start();
    }
}