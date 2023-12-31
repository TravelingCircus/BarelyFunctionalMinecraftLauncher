using System.IO;
using Common.Misc;
using BFML.Repository.RepoIOAdapters;

namespace BFML.Repository;

internal sealed class RepoIO
{
    private DirectoryInfo ForgeInfo => new DirectoryInfo(_repoInfo.FullName + "\\Forge");
    private DirectoryInfo ConfigsInfo => new DirectoryInfo(_repoInfo.FullName + "\\Configs");
    private DirectoryInfo ModPacksInfo => new DirectoryInfo(_repoInfo.FullName + "\\ModPacks");
    private DirectoryInfo ResourcesInfo => new DirectoryInfo(_repoInfo.FullName + "\\Resources");
    
    internal readonly ForgeAdapter Forge;
    internal readonly ConfigAdapter Configs;
    internal readonly ModPackAdapter ModPacks;
    internal readonly ResourceAdapter Resources;
    private readonly DirectoryInfo _repoInfo;

    internal RepoIO(DirectoryInfo repoInfo)
    {
        _repoInfo = repoInfo;
        Forge = new ForgeAdapter(ForgeInfo);
        Configs = new ConfigAdapter(ConfigsInfo);
        ModPacks = new ModPackAdapter(ModPacksInfo);
        Resources = new ResourceAdapter(ResourcesInfo);
    }
    
    internal Result<bool, InvalidDataException> Validate()
    {
        throw new System.NotImplementedException();
    }
}