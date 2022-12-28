using System.IO.Compression;
using Common;

namespace TCPFileServer.DataAccess;

public sealed class LargeDataHandler: DataHandler
{
    private readonly string _repositoryPath;
    private readonly string _forgeDirectory;
    private readonly string _modsDirectory;


    public LargeDataHandler(string repositoryPath)
    {
        _repositoryPath = repositoryPath;
        _forgeDirectory = repositoryPath + @"Forge/";
        _modsDirectory = repositoryPath + @"Mods/";
    }
    
    public uint GetModsChecksum()
    {
        DirectoryInfo directory = Directory.CreateDirectory(_repositoryPath + "/tempmodschecksum" + new Random().Next());
        ZipFile.ExtractToDirectory(new DirectoryInfo(_modsDirectory).GetFiles()[0].FullName, directory.FullName);
        uint checksum = Checksum.FromDirectory(new DirectoryInfo(directory.FullName));
        directory.Delete(true);
        return checksum;
    }
    
    public BorrowableFileStream GetForgeArchiveStream()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(_forgeDirectory);
        FileInfo[] files = directoryInfo.GetFiles();
        FileStream underlyingStream = File.OpenRead(files[0].FullName);
        return new BorrowableFileStream(underlyingStream, this);
    }

    public BorrowableFileStream GetModsArchiveStream()
    { 
        DirectoryInfo directoryInfo = new DirectoryInfo(_modsDirectory);
        FileInfo[] files = directoryInfo.GetFiles();
        FileStream underlyingStream = File.OpenRead(files[0].FullName);
        return new BorrowableFileStream(underlyingStream, this);
    }
}