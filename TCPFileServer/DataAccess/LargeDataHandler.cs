namespace HTTPFileServer.DataAccess;

public class LargeDataHandler: DataHandler
{
    private readonly string _repositoryPath;
    private string _forgeArchivePath;
    
    
    public LargeDataHandler(string repositoryPath)
    {
        _repositoryPath = repositoryPath;
    }
    
    public override Task WriteToRepository()
    {
        throw new NotImplementedException();
    }

    public BorrowableReadonlyStream GetStreamToForgeArchive()
    {
        FileStream underlyingStream = File.OpenRead(_forgeArchivePath);
        return new BorrowableReadonlyStream(underlyingStream, this);
    }
}