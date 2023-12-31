using System;
using System.IO;
using System.Threading.Tasks;
using BFML.Core;

namespace BFML.Repository.RepoIOAdapters;

internal sealed class ConfigAdapter : RepoAdapter
{
    internal ConfigAdapter(DirectoryInfo directory) : base(directory) { }

    internal Task<LocalPrefs> LoadLocalPrefs()
    {
        throw new NotImplementedException();
    }

    internal Task<bool> SaveLocalPrefs(LocalPrefs localPrefs)
    {
        throw new NotImplementedException();
    }
}