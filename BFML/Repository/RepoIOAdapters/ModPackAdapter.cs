using System;
using System.IO;
using System.Threading.Tasks;
using Common.Models;

namespace BFML.Repository.RepoIOAdapters;

internal sealed class ModPackAdapter : RepoAdapter
{
    internal ModPackAdapter(DirectoryInfo directory) : base(directory) { }
    
    internal Task<ModPackManifest> LoadManifest()
    {
        throw new NotImplementedException();
    }
    
    internal Task<bool> SaveManifest(ModPackManifest manifest)
    {
        throw new NotImplementedException();
    }
}