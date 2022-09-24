using System.Net.Sockets;
using CommonData;
using CommonData.Network;

namespace TCPFileClient;

public class FileClient
{
    private List<TaskCompletionSource<Message>> _requests;
    //TODO add cancellation token
    //TODO encapsulate TCPClient
    //TODO Channel listens endlessly on separate thread
    //TODO async connect method, that returns result (true/false)
    public static FileClient ConnectToServer()
    {
        return null;
        //return new TcpClient("127.0.0.1", 69);
    }

    public Task<string> DownloadForgeFiles()
    {
        throw new NotImplementedException();
    }

    public Task<LaunchConfiguration> DownloadLaunchConfiguration()
    {
        throw new NotImplementedException();
    }

    private Task<Message> GetResponseFor(Message request)
    {
        //Add request
        //await response data
        //handle response data
        //return response message
        throw new NotImplementedException();
    }

    private void Terminate()
    {
        throw new NotImplementedException();
    }
}