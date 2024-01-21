using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using BFML.Repository;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;
using Utils;

namespace BFML.Core;

internal sealed class Game
{
    private readonly Repo _repo;

    internal Game(Repo repo)
    {
        _repo = repo;
    }

    public async Task Launch(string nickname, GameConfiguration gameConfiguration)
    {
        MinecraftPath minecraftPath = new MinecraftPath(_repo.LocalPrefs.GameDirectory.FullName);
        CMLauncher launcher = new CMLauncher(minecraftPath);
        
        Result<MVersion> preparationResult = await PrepareConfiguration(launcher, gameConfiguration, _repo.LocalPrefs.FileValidationMode);
        if (!preparationResult.IsOk) throw preparationResult.Error; 
        
        LaunchConfiguration launchConfig = new LaunchConfiguration(
            nickname, 
            preparationResult.Value,
            _repo.LocalPrefs.DedicatedRam, 
            _repo.LocalPrefs.IsFullscreen,
            _repo.LocalPrefs.JVMLocation);
        
        await StartGameProcess(launcher, launchConfig);
    }

    private async Task<Result<MVersion>> PrepareConfiguration(CMLauncher launcher, GameConfiguration gameConfiguration, FileValidation validationMode)
    {
        Result<MVersion> vanillaPreparation = await PrepareVanilla(launcher, gameConfiguration.Vanilla.ToString());
        if (!vanillaPreparation.IsOk) return Result<MVersion>.Err(vanillaPreparation.Error);

        if (!gameConfiguration.IsModded) return Result<MVersion>.Ok(vanillaPreparation.Value);
        
        Result<MVersion> forgePreparation = await PrepareForge(launcher, gameConfiguration.Forge, validationMode);
        if (!forgePreparation.IsOk) return Result<MVersion>.Err(forgePreparation.Error);

        if (gameConfiguration.ModPack == null) return Result<MVersion>.Ok(forgePreparation.Value);
        
        return (await PrepareMods(gameConfiguration.ModPack, validationMode))
            .Match(ok => Result<MVersion>.Ok(forgePreparation.Value), Result<MVersion>.Err);
    }

    private static async Task<Result<MVersion>> PrepareVanilla(CMLauncher launcher, string versionName)
    {
        if (string.IsNullOrWhiteSpace(versionName))
        {
            return Result<MVersion>.Err(new InvalidDataException("Version name is empty."));
        }

        try
        {
            MVersion vanilla = await launcher.GetVersionAsync(versionName);
            await launcher.CheckAndDownloadAsync(vanilla); // TODO track progress and time out
            return vanilla;
        }
        catch (Exception e)
        {
            return Result<MVersion>.Err(e);
        }
    }

    private async Task<Result<MVersion>> PrepareForge(CMLauncher launcher, Forge forge, FileValidation validationMode)
    {
        Result<bool> installation = await _repo.InstallForge(forge, validationMode);
        if (!installation.IsOk) return Result<MVersion>.Err(installation.Error);

        try
        {
            MVersionCollection localVersions = await new LocalVersionLoader(launcher.MinecraftPath).GetVersionMetadatasAsync();
            MVersion forgeVersion = await localVersions.GetVersionAsync(forge.Name);
            return Result<MVersion>.Ok(forgeVersion);
        }
        catch (Exception e)
        {
            return Result<MVersion>.Err(e);
        }
    }

    private Task<Result<bool>> PrepareMods(ModPack modPack, FileValidation validationMode)
    {
        return _repo.InstallModPack(modPack, validationMode);
    }
    
    private static async Task StartGameProcess(CMLauncher launcher, LaunchConfiguration launchConfig)
    {
        System.Net.ServicePointManager.DefaultConnectionLimit = 256;
        Process process = await launcher.CreateProcessAsync(launchConfig.Version, new MLaunchOption
        {
            MaximumRamMb = launchConfig.DedicatedRam,
            Session = MSession.CreateOfflineSession(launchConfig.Nickname),
            FullScreen = launchConfig.FullScreen,
            JavaPath = launchConfig.JVMLocation.FullName
        }, false);
        process.Start();
    }
}