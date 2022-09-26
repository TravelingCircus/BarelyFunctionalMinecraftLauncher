namespace TCPFileServer.DataAccess;

public sealed class LargeDataHandler: DataHandler
{
    private readonly string _repositoryPath;
    private readonly string _forgeArchivePath;
    private readonly string _modsArchivePath;


    public LargeDataHandler(string repositoryPath)
    {
        _repositoryPath = repositoryPath;
        _forgeArchivePath = repositoryPath + @"Forge\";
        _modsArchivePath = repositoryPath + @"Mods\";
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