using System.Threading.Tasks;
using BFML.Core;
using Common;
using Common.Models;

namespace BFML.Repository;

internal abstract class Repo
{
    internal LocalPrefs LocalPrefs => _localPrefs;
    private LocalPrefs _localPrefs;
    protected readonly RepoIO RepoIo;

    protected Repo(RepoIO repoIo)
    {
        RepoIo = repoIo;
    }

    public virtual async Task<bool> TryInit()
    {
        _localPrefs = await RepoIo.Configs.LoadLocalPrefs();

        return true;
    }
    
    internal abstract Task<Forge[]> LoadForgeList();

    internal abstract Task<ModPackManifest[]> LoadModPackManifestList();

    internal Task<Skin> LoadDefaultSkin() => RepoIo.Resources.LoadDefaultSkin();

    internal async Task<bool> SaveLocalPrefs(LocalPrefs localPrefs)
    {
        bool success = await RepoIo.Configs.SaveLocalPrefs(localPrefs);
        if (success) _localPrefs = localPrefs;
        return success;
    }

    internal async Task<bool> ClearLocalPrefs()
    {
        bool success = await RepoIo.Configs.ClearLocalPrefs();
        if(success) _localPrefs = new LocalPrefs();
        return success;
    }
}