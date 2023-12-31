using System;
using System.IO;
using System.Threading.Tasks;
using Common;

namespace BFML.Repository.RepoIOAdapters;

internal sealed class ResourceAdapter : RepoAdapter
{
    internal ResourceAdapter(DirectoryInfo directory) : base(directory) { }
    
    internal Task<FileInfo> LoadFont()
    {
        throw new NotImplementedException();
    }
    
    internal Task<Skin> LoadDefaultSkin()
    {
        throw new NotImplementedException();
    }
}