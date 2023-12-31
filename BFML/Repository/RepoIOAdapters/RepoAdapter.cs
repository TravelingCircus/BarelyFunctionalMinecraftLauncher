using System.IO;

namespace BFML.Repository.RepoIOAdapters;

internal abstract class RepoAdapter
{
    protected readonly DirectoryInfo AdapterDirectory;
    
    protected RepoAdapter(DirectoryInfo directory)
    {
        AdapterDirectory = directory;
    }
}