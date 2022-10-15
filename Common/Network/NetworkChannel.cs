using System.Net.Sockets;

namespace Common.Network;

public sealed class NetworkChannel
{
    private readonly NetworkStream _stream;
    
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
        int read = 0;
        List<byte> result = new List<byte>();
        while (read < header.DataLength)
        {
            int leftToRead = header.DataLength - read;
            byte[] buffer = new byte[leftToRead];
            int readThisTime = _stream.Read(buffer, 0, leftToRead);
            result.AddRange(buffer[0..readThisTime]);
            read += readThisTime;
        }
        memoryStream.Write(result.ToArray(), 0, header.DataLength);
        memoryStream.Position = 0;
        return Task.FromResult((Stream)memoryStream);
    }
    
    public Task SendMessage(Message message)
    {
        _stream.Write(message.GetHeader().ToByteArray());
        return message.WriteDataTo(_stream);
    }
}