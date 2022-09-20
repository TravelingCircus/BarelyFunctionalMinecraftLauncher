using System.Net.Sockets;
using CommonData;
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

        DownloadFile(stream, @"D:\Home\Desktope\TestDownload\ExtractedForgeFiles1.16.5.zip");

        client.Close();
        Console.WriteLine("Client closed");
        
        return new BFMLFileClient();
    }
    
    public static BFMLFileClient TransferUserToServer(User user)
    {
        TcpClient client = new TcpClient("127.0.0.1", 7000);
        Console.WriteLine("Client connected");
        
        NetworkStream stream = client.GetStream();

        byte[] bytesToWrite = DataConverter.ObjectToByteArray(user);
        stream.Write(bytesToWrite, 0, bytesToWrite.Length);

        client.Close();
        Console.WriteLine("Client closed");
        
        return new BFMLFileClient();
    }

    private static void DownloadFile(NetworkStream stream, string path)
    {
        byte[] bytesRead = DataConverter.ReadAllBytesFromStream(stream);
        
        using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate))
        {
            fileStream.Write(bytesRead, 0, bytesRead.Length);
        }
    }

    public Task<string> DownloadForgeFiles()
    {
        throw new NotImplementedException();
    }

    public Task<LaunchConfiguration> DownloadLaunchConfiguration()
    {
        throw new NotImplementedException();
    }
}