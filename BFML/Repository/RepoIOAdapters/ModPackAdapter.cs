using System;
using System.IO;
using System.Threading.Tasks;
using BFML.Core;
using Common.Models;

namespace BFML.Repository.RepoIOAdapters;

internal sealed class ModPackAdapter : RepoAdapter
{
    internal ModPackAdapter(DirectoryInfo directory) : base(directory) { }
    
    internal Task<ModPack[]> LoadModPackList()
    {
        throw new NotImplementedException();
    }
}