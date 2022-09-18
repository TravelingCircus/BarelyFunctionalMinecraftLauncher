using FileClient.Utils;

namespace FileClient;

public class BFMLFileClient
{
    private const string ForgeFilesDirectory = "ForgeFiles";
    private const string TargetDirectory = "Target";
    
    public static BFMLFileClient ConnectToServer()
    {
        return new BFMLFileClient();
        //TODO actually connect to the server
    }

    public Task DownloadForgeFiles(string savePath)
    {
        DirectoryInfo source = new DirectoryInfo(ForgeFilesDirectory);
        source.CopyTo(TargetDirectory);
        return Task.CompletedTask;
    }
}