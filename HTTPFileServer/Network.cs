using System.Net;
using System.Net.Sockets;
using CommonData;

namespace HTTPFileServer;

public sealed class Network : IDisposable
{
    private TcpListener _serverSocket;
    
    public void Start()
    {
        _serverSocket = new TcpListener(IPAddress.Any, 7000);
        _serverSocket.Start();
        Console.WriteLine("Server started");
    }

    public byte[] ListenForBytes()
    {
        TcpClient client = ListenForConnection();
        NetworkStream stream = client.GetStream();
        byte[] bytes = DataConverter.ReadAllBytesFromStream(stream);
        stream.Flush();
        client.Close();
        return bytes;
    }
    
    public TcpClient ListenForConnection()
    {
        return _serverSocket.AcceptTcpClient();
    }

    public void Dispose()
    {
        _serverSocket.Stop();
        Console.WriteLine("Server stopped");
    }
}