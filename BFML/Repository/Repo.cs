using System.Threading.Tasks;
using BFML.Core;
using Common;
using Common.Models;

namespace BFML.Repository;

internal abstract class Repo
{
    protected LocalPrefs LocalPrefs;
    protected readonly RepoIO RepoIo;

    protected Repo(RepoIO repoIo)
    {
        RepoIo = repoIo;
    }
    
    internal abstract Task<Forge[]> LoadForgeList();

    internal abstract Task<ModPackManifest[]> LoadModPackManifestList();

    internal Task<Skin> LoadDefaultSkin() => RepoIo.Resources.LoadDefaultSkin();
    
    internal async Task<LocalPrefs> LoadLocalPrefs()
    {
        LocalPrefs ??= await RepoIo.Configs.LoadLocalPrefs();
        return LocalPrefs;
    }

    internal async Task<bool> SaveLocalPrefs(LocalPrefs localPrefs)
    {
        bool operationResult = await RepoIo.Configs.SaveLocalPrefs(localPrefs);
        if(operationResult) LocalPrefs = localPrefs;
        return operationResult;
    }

    internal async Task<bool> ClearLocalPrefs()
    {
        bool operationResult = await RepoIo.Configs.ClearLocalPrefs();
        if(operationResult) LocalPrefs = new LocalPrefs();
        return operationResult;
    }
}