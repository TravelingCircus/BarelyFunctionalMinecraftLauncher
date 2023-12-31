using System.Threading.Tasks;
using BFML.Core;
using Common.Models;
using FileClient;

namespace BFML.Repository;

internal sealed class RemoteModeRepo : Repo
{
    private readonly ServerConnection _serverConnection;
    
    internal RemoteModeRepo(RepoIO repoIo, ServerConnection serverConnection) : base(repoIo)
    {
        _serverConnection = serverConnection;
    }

    internal override Task<Forge[]> LoadForgeList()
    {
        throw new System.NotImplementedException();
    }

    internal override Task<ModPackManifest[]> LoadModPackManifestList()
    {
        throw new System.NotImplementedException();
    }
}