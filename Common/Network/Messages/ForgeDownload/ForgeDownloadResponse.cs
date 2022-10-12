namespace CommonData.Network.Messages;

public class ForgeDownloadResponse : Message
{
    public string TempForgePath;
    public int ForgeBytesLength;
    public byte[] ForgeBytes;
    private Stream _stream;
    
    public ForgeDownloadResponse()
    {
        
    }

    public ForgeDownloadResponse(Stream stream, int forgeBytesLength)
    {
        _stream = stream;
        ForgeBytesLength = forgeBytesLength;
    }
    
    public ForgeDownloadResponse(int forgeBytesLength, byte[] forgeBytes)
    {
        ForgeBytesLength = forgeBytesLength;
        ForgeBytes = forgeBytes;
    }

    public override MessageHeader GetHeader()
    {
        return new MessageHeader(MessageRegistry.GetKeyForMessageType(typeof(ForgeDownloadResponse)), 
            sizeof(int) + ForgeBytesLength);
    }
    
    public override async Task WriteDataTo(Stream targetStream)
    {
        WriteToStream(targetStream, ForgeBytesLength);
        byte[] buffer = new byte[67108864];
        int lastRead = 0;
        WriteToStream(targetStream, ForgeBytesLength);
        do
        {
            lastRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
            await targetStream.WriteAsync(buffer, 0, lastRead);
        } while (lastRead >= buffer.Length);
        await _stream.FlushAsync();
        _stream.Close();
    }

    public override void ApplyData(Stream stream)
    {
        ForgeBytesLength = IntReadStream(stream);
        ForgeBytes = ByteArrayReadStream(stream, ForgeBytesLength);
    }
    
    protected override Stream GetData()
    {
        throw new NotSupportedException();
    }
}