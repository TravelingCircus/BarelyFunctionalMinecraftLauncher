using FileClient.Utils;

namespace FileClient;

public class BFMLFileClient
{
    private const string ForgeFilesDirectory = "ForgeFiles";

    public static BFMLFileClient ConnectToServer()
    {
        return new BFMLFileClient();
        //TODO actually connect to the server
    }

    public Task DownloadForgeFiles(string savePath)
    {
        return Task.Run(() =>
        {
            DirectoryInfo source = new DirectoryInfo(ForgeFilesDirectory);
            source.CopyTo(savePath, true);
        });
    }
}