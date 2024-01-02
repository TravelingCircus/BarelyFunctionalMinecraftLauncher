using System.Threading.Tasks;
using BFML.Core;
using BFML.Repository.RepoIOAdapters;
using Common.Misc;
using Common.Models;
using FileClient;
using Utils;

namespace BFML.Repository;

internal sealed class CentralizedModeRepo : Repo
{
    private readonly ServerConnection _serverConnection;
    
    internal CentralizedModeRepo(RepoIO repoIo, ServerConnection serverConnection) : base(repoIo)
    {
        _serverConnection = serverConnection;
    }

    internal Task<Result<User>> CreateRecord(User user) => _serverConnection.CreateRecord(user);

    internal Task<Result<User>> Authenticate(User user) => _serverConnection.Authenticate(user);

    protected override bool ForgeFilter(Forge forge)
    {
        return forge.Name == ForgeAdapter.CentralizedReservedForge;
    }

    protected override bool ModPackFilter(ModPack modPack)
    {
        return modPack.Name == ModPackAdapter.CentralizedReservedModPack;
    }
}