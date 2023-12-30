using Common;
using Common.Misc;
using Common.Models;

namespace LocalFileClient;

public class FileClient : IFileClient
{
    /* LOCAL REPOSITORY STRUCTURE
     *
     *  BFML.exe
     *  Repo
     *  |->Configs
     *     |->LaunchConfiguration.xml
     *     |->ConfigVersion.xml
     *     |->LocalPrefs.xml
     *  |->Forge                             // forge file archives
     *     |->1.18.2-forge-40.2.0.zip        // forge version name 
     *        |->1.18.2-forge-40.2.0         // forge version directory
     *        |->libraries                   // forge file extraction: https://alphabs.gitbook.io/cmllib/cmllib.core/installer/forge-installer            
     *  |->ModPacks
     *     |->ModPack1                       // mod pack name
     *        |->ModPack1.xml                // mod pack descriptor file (vanilla version, forge version, etc...)
     *        |->Mods.zip                    // contents of this archive are copied to game's `mods` directory
     *           |->1.7.10flan_mod.jar       
     *           |->mod-name.jar
     *  
     */
    
    private readonly FileInfo _configVersionFile = new FileInfo("");//TODO INSERT ACTUAL PATH
    private readonly FileInfo _launchConfigFile = new FileInfo("");//TODO INSERT ACTUAL PATH
    private readonly CancellationTokenSource _cancellationSource = new();

    public Task<bool> TryInit()
    {
        return Task.FromResult(ValidateResources().IsOk);
    }

    public Task<bool> TryDispose()
    {
        _cancellationSource.Cancel();
        return Task.FromResult(true);
    }

    public Task<Result<User>> Authenticate(User user)
    {
        // load user from local file
        throw new NotImplementedException();
    }

    public Task<Result<User>> CreateRecord(User user)
    {
        //rewrite local file
        throw new NotImplementedException();
    }

    public Task<ConfigurationVersion> LoadConfigVersion()
    {
        throw new NotImplementedException();
    }

    public Task<LaunchConfiguration> LoadLaunchConfiguration()
    {
        throw new NotImplementedException();
    }

    public Task<bool> TryLoadForge(FileInfo target)
    {
        throw new NotImplementedException();
    }

    public Task<bool> TryLoadMods(FileInfo target)
    {
        throw new NotImplementedException();
    }

    public Task<bool> TryChangeSkin(User user, Skin skin)    
    {
        throw new NotImplementedException();
    }

    public Task<Skin> GetSkin(User user)
    {
        throw new NotImplementedException();
    }

    private Result<bool, IOException> ValidateResources()
    {
        if (!_configVersionFile.Exists)
        {
            return Result<bool, IOException>.Err(
                new IOException($"Config version file does not exist at: {_configVersionFile.FullName}"));
        }
        if (!_launchConfigFile.Exists)
        {
            return Result<bool, IOException>.Err(
                new IOException($"Launch config file does not exist at: {_launchConfigFile.FullName}"));
        }
        return Result<bool, IOException>.Ok(true);
    }
}