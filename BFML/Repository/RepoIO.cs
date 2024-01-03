﻿using System.IO;
using Common.Misc;
using BFML.Repository.RepoIOAdapters;
using Utils;

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
        Forge = new ForgeAdapter(ForgeDirectory, this);
        Configs = new ConfigAdapter(ConfigsDirectory, this);
        ModPacks = new ModPackAdapter(ModPacksDirectory, this);
        Resources = new ResourceAdapter(ResourcesDirectory, this);
    }
    
    internal Result<bool, InvalidDataException> Validate()
    {
        return _repoInfo.Exists 
               && ForgeDirectory.Exists 
               && ConfigsDirectory.Exists 
               && ModPacksDirectory.Exists 
               && ResourcesDirectory.Exists;
    }
}