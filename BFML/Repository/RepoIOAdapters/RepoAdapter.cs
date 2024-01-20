using System.IO;

namespace BFML.Repository.RepoIOAdapters;

internal abstract class RepoAdapter
{
    protected readonly DirectoryInfo AdapterDirectory;
    protected readonly RepoIO RepoIo;
    
    protected RepoAdapter(DirectoryInfo directory, RepoIO repoIo)
    {
        AdapterDirectory = directory;
        RepoIo = repoIo;
    }
}