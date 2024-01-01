using System;
using System.IO;
using System.Threading.Tasks;
using Common.Models;

namespace BFML.Repository.RepoIOAdapters;

internal sealed class ModPackAdapter : RepoAdapter
{
    internal ModPackAdapter(DirectoryInfo directory) : base(directory) { }
    
    internal Task<ModPackManifest[]> LoadModPackManifestList()
    {
        throw new NotImplementedException();
    }
    
    internal Task<bool> SaveModPackManifest(ModPackManifest manifest)
    {
        throw new NotImplementedException();
    }
}