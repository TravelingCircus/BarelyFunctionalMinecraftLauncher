using System.Threading.Tasks;
using BFML.Core;
using Common.Misc;
using Common.Models;
using FileClient;

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

    internal override Task<Forge[]> LoadForgeList()
    {
        throw new System.NotImplementedException();
    }

    internal override Task<ModPackManifest[]> LoadModPackManifestList()
    {
        throw new System.NotImplementedException();
    }
}