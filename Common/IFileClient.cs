using Common.Misc;
using Common.Models;
namespace Common;

public interface IFileClient
{
    public Task<bool> TryInit();
    public Task<bool> TryDispose();
    
    public Task<Result<User>> Authenticate(User user);
    public Task<Result<User>> CreateRecord(User user);
    
    public Task<ConfigurationVersion> LoadConfigVersion();
    public Task<LaunchConfiguration> LoadLaunchConfiguration();
    
    public Task<bool> TryLoadForge(FileInfo target);
    public Task<bool> TryLoadMods(FileInfo target);
    
    public Task<bool> TryChangeSkin(string filePath);
    public Task<Skin> GetSkin();
}