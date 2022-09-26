namespace HTTPFileServer.DataAccess;

public abstract class DataHandler
{
    private TaskCompletionSource _releaseSource;
    
    public void Borrow(TaskCompletionSource releaseSource)
    {
        _releaseSource = releaseSource;
    }
    
    public void Release()
    {
        if (_releaseSource is null) throw new NullReferenceException("Trying to release handler that was never borrowed");
        _releaseSource.SetResult();
    }

    protected Stream ReadFromRepository(string repositoryPath, string fileName)
    {
        string filePath = repositoryPath + fileName;
        FileStream fileStream = new FileStream(filePath, FileMode.Open);
        fileStream.Position = 0;
        return fileStream;
    }
    
    public byte[] ReadFromRepository(string filePath)
    {
        return ReadFromRepository(filePath, GetFileSize(filePath));
    }
    
    protected byte[] ReadFromRepository(string filePath, int fileSize)
    {
        using FileStream fileStream = new FileStream(filePath, FileMode.Open);
        byte[] fileBytes = new byte[fileSize];
        int bytesRead = fileStream.Read(fileBytes, 0, fileSize);
        if (bytesRead != fileSize) throw new IOException($"Read {bytesRead} from {fileSize}");
        return fileBytes;
    }
    
    protected int GetFileSize(string path)
    {
        long size = new FileInfo(path).Length;
        if (size > Int32.MaxValue) throw new OverflowException("Can't send files larger than 2GB");
        return (int)size;
    }
}