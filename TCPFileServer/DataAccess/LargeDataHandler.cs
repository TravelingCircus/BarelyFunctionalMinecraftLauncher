namespace HTTPFileServer.DataAccess;

public class LargeDataHandler: DataHandler
{
    private readonly string _repositoryPath;
    private string _forgeArchivePath;
    private string _modsArchivePath;


    public LargeDataHandler(string repositoryPath)
    {
        _repositoryPath = repositoryPath;
    }
    
    public BorrowableReadonlyStream GetForgeArchiveStream()
    {
        FileStream underlyingStream = File.OpenRead(_forgeArchivePath);
        return new BorrowableReadonlyStream(underlyingStream, this);
    }

    public BorrowableReadonlyStream GetModsArchiveStream()
    {
        FileStream underlyingStream = File.OpenRead(_modsArchivePath);
        return new BorrowableReadonlyStream(underlyingStream, this);
    }
}