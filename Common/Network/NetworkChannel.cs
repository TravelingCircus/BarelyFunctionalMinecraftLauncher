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

    public async Task<MessageHeader> ListenForHeader()
    {
        byte[] byteHeader = new byte[5];
        int bytesRead = 0;
        while (bytesRead != 5)
        {
            bytesRead = await _stream.ReadAsync(byteHeader, 0, 5);
        }
        return new MessageHeader(byteHeader[0], BitConverter.ToInt32(byteHeader, 1));
    }
    
    public Task<Stream> ListenForMessage(MessageHeader header)
    {
        MemoryStream memoryStream = new MemoryStream(header.DataLength);
        _stream.CopyToAsync(memoryStream);
        return Task.FromResult((Stream)memoryStream);
    }
    
    public Task SendMessage(Message message)
    {
        _stream.Write(message.GetHeader().ToByteArray());
        return message.WriteDataTo(_stream);
    }
}