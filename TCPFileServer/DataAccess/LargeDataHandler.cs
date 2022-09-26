namespace HTTPFileServer.DataAccess;

public class LargeDataHandler: DataHandler
{
    private readonly string _repositoryPath;
    
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
        throw new NotImplementedException();
    }
}