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

    public Task<MessageHeader> ListenForHeader()
    {
        throw new NotImplementedException();
    }
    
    public Task<Stream> ListenForMessage(MessageHeader header)
    {
        throw new NotImplementedException();
    }
    
    public Task SendMessage(Message message)
    {
        throw new NotImplementedException();
    }
}