using System;
using System.IO;
using System.Threading.Tasks;
using BFML.Core;
using Common.Models;

namespace BFML.Repository.RepoIOAdapters;

internal sealed class ModPackAdapter : RepoAdapter
{
    public const string CentralizedReservedModPack = "CentralizedModpack";
    
    internal ModPackAdapter(DirectoryInfo directory, RepoIO repoIo) : base(directory, repoIo) { }
    
    internal Task<ModPack[]> LoadModPackList()
    {
        throw new NotImplementedException();
    }
}