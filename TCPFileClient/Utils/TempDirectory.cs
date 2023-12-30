namespace FileClient.Utils;

public sealed class TempDirectory : IDisposable
{
    public readonly DirectoryInfo Info;

    public TempDirectory()
    {
        Info = CreateTempDirectory();
    }

    public void Dispose()
    {
        Info.Delete(true);
    }

    private static DirectoryInfo CreateTempDirectory()
    {
        string path = Path.GetTempPath() 
                      + "BFMLTemp" 
                      + new Random().Next(9999);

        return Directory.CreateDirectory(path);
    }
}