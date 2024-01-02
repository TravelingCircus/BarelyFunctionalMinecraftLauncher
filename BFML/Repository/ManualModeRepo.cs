using System.Threading.Tasks;
using BFML.Core;
using Common;
using Common.Models;

namespace BFML.Repository;

internal sealed class ManualModeRepo : Repo
{
    internal ManualModeRepo(RepoIO repoIo) : base(repoIo) { }
    public Skin DefaultSkin { get; set; }

    internal override Task<Forge[]> LoadForgeList() => RepoIo.Forge.LoadForgeList();

    internal override Task<ModPack[]> LoadModPackList() => RepoIo.ModPacks.LoadModPackList();
}