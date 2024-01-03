﻿using System.Linq;
using System.Threading.Tasks;
using BFML.Core;
using Common;
using Utils;

namespace BFML.Repository;

internal abstract class Repo
{
    internal LocalPrefs LocalPrefs { get; private set; }
    protected readonly RepoIO RepoIo;

    protected Repo(RepoIO repoIo)
    {
        RepoIo = repoIo;
    }

    public virtual async Task<bool> TryInit()
    {
        Result<LocalPrefs> loadResult = await RepoIo.Configs.LoadLocalPrefs();
        if (loadResult.IsOk) LocalPrefs = loadResult.Value; 
        return loadResult.IsOk;
    }

    protected abstract bool ForgeFilter(Forge forge);
    
    internal async Task<Forge[]> LoadForgeList()
    {
        return (await RepoIo.Forge.LoadForgeList())
            .Where(ForgeFilter)
            .ToArray();
    }

    protected abstract bool ModPackFilter(ModPack modPack);

    internal async Task<ModPack[]> LoadModPackList()
    {
        return (await RepoIo.ModPacks.LoadModPackList())
            .Where(ModPackFilter)
            .ToArray();
    }

    internal Task<Skin> LoadDefaultSkin() => RepoIo.Resources.LoadDefaultSkin();

    internal async Task<bool> SaveLocalPrefs(LocalPrefs localPrefs)
    {
        bool success = await RepoIo.Configs.SaveLocalPrefs(localPrefs);
        if (success) LocalPrefs = localPrefs;
        return success;
    }

    internal async Task<bool> ClearLocalPrefs()
    {
        bool success = await RepoIo.Configs.ClearLocalPrefs();
        if(success) LocalPrefs = new LocalPrefs();
        return success;
    }
}