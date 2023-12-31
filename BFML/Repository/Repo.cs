using System.Threading.Tasks;
using BFML.Core;
using Common;
using Common.Models;

namespace BFML.Repository;

internal abstract class Repo
{
    protected readonly RepoIO RepoIo;

    protected Repo(RepoIO repoIo)
    {
        RepoIo = repoIo;
    }
    
    internal abstract Task<Forge[]> LoadForgeList();

    internal abstract Task<ModPackManifest[]> LoadModPackManifestList();

    internal Task<LocalPrefs> LoadLocalPrefs() => RepoIo.Configs.LoadLocalPrefs();
    internal Task<Skin> LoadDefaultSkin() => RepoIo.Resources.LoadDefaultSkin();
}