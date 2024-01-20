using BFML.Core;
using BFML.Repository.RepoIOAdapters;
using Common;

namespace BFML.Repository;

internal sealed class ManualModeRepo : Repo
{
    internal ManualModeRepo(RepoIO repoIo) : base(repoIo) { }

    protected override bool ForgeFilter(Forge forge)
    {
        return forge.Name != ForgeAdapter.CentralizedReservedForge;
    }

    protected override bool ModPackFilter(ModPack modPack)
    {
        return modPack.Name != ModPackAdapter.CentralizedReservedModPack;
    }
}