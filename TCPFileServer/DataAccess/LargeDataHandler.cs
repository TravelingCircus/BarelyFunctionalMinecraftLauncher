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
    
    public BorrowableFileStream GetForgeArchiveStream()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(_forgeArchivePath);
        FileInfo[] files = directoryInfo.GetFiles();
        FileStream underlyingStream = File.OpenRead(files[0].FullName);
        return new BorrowableFileStream(underlyingStream, this);
    }

    public BorrowableFileStream GetModsArchiveStream()
    {
        FileStream underlyingStream = File.OpenRead(_modsArchivePath);
        return new BorrowableFileStream(underlyingStream, this);
    }
}