using System.Net.Sockets;
using FileClient.Utils;

namespace FileClient;

public class BFMLFileClient
{
    private const string ForgeFilesDirectory = "ForgeFiles";

    public static BFMLFileClient ConnectToServer()
    {
        TcpClient client = new TcpClient("127.0.0.1", 7000);
        Console.WriteLine("Client connected");
        
        NetworkStream stream = client.GetStream();

        byte[] bytesRead = ReadAllBytesFromStream(stream);
        
        using (FileStream fileStream = new FileStream(@"D:\Home\Desktope\TestDownload\ExtractedForgeFiles1.16.5.zip", FileMode.OpenOrCreate))
        {
            fileStream.Write(bytesRead, 0, bytesRead.Length);
        }

        client.Close();
        Console.WriteLine("Client closed");
        
        return new BFMLFileClient();
        //TODO actually connect to the server
    }

    private static byte[] ReadAllBytesFromStream(NetworkStream networkStream)
    {
        byte[] data = new byte[32768];
        byte[] buffer;
        using (MemoryStream memoryStream = new MemoryStream())
        {
            int read;
            while ((read = networkStream.Read(data, 0, data.Length)) > 0)
            {
                memoryStream.Write(data, 0, read);
            }

            buffer = memoryStream.ToArray();
        }

        return buffer;
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