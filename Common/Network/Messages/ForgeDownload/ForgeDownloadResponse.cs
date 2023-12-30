namespace Common.Network.Messages.ForgeDownload;

public sealed class ForgeDownloadResponse : ZipFileMessage
{
    public byte[] ForgeBytes;
    private int _forgeBytesLength;
    private readonly Stream _stream;
    
    public ForgeDownloadResponse() { }

    public ForgeDownloadResponse(Stream stream, int forgeBytesLength)
    {
        _stream = stream;
        _forgeBytesLength = forgeBytesLength;
    }

    public override MessageHeader GetHeader()
    {
        return new MessageHeader(MessageRegistry.GetKeyForMessageType(typeof(ForgeDownloadResponse)), 
            sizeof(int) + _forgeBytesLength);
    }

    public override void ApplyData(Stream stream)
    {
        _forgeBytesLength = IntReadStream(stream);
        ForgeBytes = ByteArrayReadStream(stream, _forgeBytesLength);
    }
    
    protected override Stream GetData()
    {
        throw new NotSupportedException();
    }
    
    public override int GetDataLength() => _forgeBytesLength;

    protected override Stream GetDataStream() => _stream;
}