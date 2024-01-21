using BFML.Core;
using BFML.Repository.RepoIOAdapters;

namespace BFML.Repository;

internal sealed class CentralizedModeRepo : Repo
{
    //private readonly ServerConnection _serverConnection;
    
    internal CentralizedModeRepo(RepoIO repoIo) : base(repoIo) //ServerConnection serverConnection
    {
        //_serverConnection = serverConnection;
    }

    //internal Task<Result<User>> CreateRecord(User user) => _serverConnection.CreateRecord(user);

    //internal Task<Result<User>> Authenticate(User user) => _serverConnection.Authenticate(user);

    protected override bool ForgeFilter(Forge forge)
    {
        return forge.Name == ForgeAdapter.CentralizedReservedForge;
    }

    protected override bool ModPackFilter(ModPack modPack)
    {
        return modPack.Name == ModPackAdapter.CentralizedReservedModPack;
    }
}