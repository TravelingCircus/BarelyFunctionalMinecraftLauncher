namespace TCPFileClient.Utils;

public sealed class TempDirectory : IDisposable
{
    public readonly DirectoryInfo Info;

    public TempDirectory()
    {
        Info = CreateTempDirectory();
    }

    private DirectoryInfo CreateTempDirectory()
    {
        string path = Path.GetTempPath() 
                      + "BFMLTemp" 
                      + new Random().Next(9999);

        return Directory.CreateDirectory(path);
    }
    
    public void Dispose()
    {
        Info.Delete(true);
    }
}