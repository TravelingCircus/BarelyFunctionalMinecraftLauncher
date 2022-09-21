using System.Net.Sockets;
using CommonData.Network;

namespace CommonData;

public class NetworkChannel
{
    private NetworkStream _stream;
    
    public NetworkChannel(NetworkStream stream)
    {
        _stream = stream;
    }
    
    public Task SendMessage(Message message)
    {
        throw new NotImplementedException();
    }

    public Task<Message> ListenForMessage()
    {
        throw new NotImplementedException();
    }
}