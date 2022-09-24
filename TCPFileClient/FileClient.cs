using System.Net.Sockets;
using CommonData;

namespace TCPFileClient;

public class FileClient
{
    public static TcpClient ConnectToServer()
    {
        TcpClient client = new TcpClient("127.0.0.1", 69);
        Console.WriteLine("Client connected");
        return client;
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