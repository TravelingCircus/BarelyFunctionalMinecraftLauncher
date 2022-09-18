namespace FileClient.Utils;

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
                      + new Random().Next() % 100;

        return Directory.CreateDirectory(path);
    }
    
    public void Dispose()
    {
        Directory.Delete(Info.FullName);
    }
}