using System.Net.Sockets;
using CommonData;
using FileClient.Requests;

namespace FileClient;

public class BFMLFileClient
{
    private const string ForgeFilesDirectory = "ForgeFiles";
    
    public static TcpClient DebugConnectToServer()
    {
        TcpClient client = new TcpClient("127.0.0.1", 7000);
        Console.WriteLine("Client connected");
        return client;
    }

    public static NetworkStream GetServerStream(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        return stream;
    }
    
    public static void DebugClientRequest(NetworkStream stream, string message)
    {
        Request.SendTextMessage(stream, message);
    }
    
    public static void DebugDisconnectFromServer(TcpClient client)
    {
        client.Close();
        Console.WriteLine("Client closed");
    }
    
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