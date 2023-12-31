using System.IO;
using Common.Misc;
using BFML.Repository.RepoIOAdapters;

namespace BFML.Repository;

internal sealed class RepoIO
{
    private DirectoryInfo ForgeDirectory => new DirectoryInfo(_repoInfo.FullName + "\\Forge");
    private DirectoryInfo ConfigsDirectory => new DirectoryInfo(_repoInfo.FullName + "\\Configs");
    private DirectoryInfo ModPacksDirectory => new DirectoryInfo(_repoInfo.FullName + "\\ModPacks");
    private DirectoryInfo ResourcesDirectory => new DirectoryInfo(_repoInfo.FullName + "\\Resources");
    
    internal readonly ForgeAdapter Forge;
    internal readonly ConfigAdapter Configs;
    internal readonly ModPackAdapter ModPacks;
    internal readonly ResourceAdapter Resources;
    private readonly DirectoryInfo _repoInfo;

    internal RepoIO(DirectoryInfo repoInfo)
    {
        _repoInfo = repoInfo;
        Forge = new ForgeAdapter(ForgeDirectory);
        Configs = new ConfigAdapter(ConfigsDirectory);
        ModPacks = new ModPackAdapter(ModPacksDirectory);
        Resources = new ResourceAdapter(ResourcesDirectory);
    }
    
    internal Result<bool, InvalidDataException> Validate()
    {
        throw new System.NotImplementedException();
    }
}