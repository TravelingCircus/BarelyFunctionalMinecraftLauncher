using System.Threading.Tasks;
using BFML.Core;
using Common.Models;

namespace BFML.Repository;

internal sealed class LocalModeRepo : Repo
{
    internal LocalModeRepo(RepoIO repoIo) : base(repoIo) { }

    internal override Task<Forge[]> LoadForgeList() => RepoIo.Forge.LoadForgeList();

    internal override Task<ModPackManifest[]> LoadModPackManifestList() => RepoIo.ModPacks.LoadModPackManifestList();
}