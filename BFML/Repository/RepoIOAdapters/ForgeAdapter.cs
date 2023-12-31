using System;
using System.IO;
using System.Threading.Tasks;
using BFML.Core;

namespace BFML.Repository.RepoIOAdapters;

internal sealed class ForgeAdapter : RepoAdapter
{
    internal ForgeAdapter(DirectoryInfo directory) : base(directory) { }

    internal Task<Forge[]> LoadForgeList()
    {
        throw new NotImplementedException();
    }
    
    internal Task<bool> SaveForge(Forge forge)
    {
        throw new NotImplementedException();
    }
}