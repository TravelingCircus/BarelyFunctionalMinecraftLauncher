namespace Common.Network.Messages.ModsDownload;

public sealed class ModsDownloadResponse: ZipFileMessage
{
    public string ModsZipPath;
    public byte[] ModsBytes;
    private int _modsBytesLength;
    private readonly Stream _stream;
    
    public ModsDownloadResponse() { }

    public ModsDownloadResponse(Stream stream, int modsBytesLength)
    {
        _stream = stream;
        _modsBytesLength = modsBytesLength;
    }
    
    public override MessageHeader GetHeader()
    {
        return new MessageHeader(MessageRegistry.GetKeyForMessageType(typeof(ModsDownloadResponse)), 
            sizeof(int) + _modsBytesLength);
    }

    public override void ApplyData(Stream stream)
    {
        _modsBytesLength = IntReadStream(stream);
        ModsBytes = ByteArrayReadStream(stream, _modsBytesLength);
    }

    protected override Stream GetData()
    {
        throw new NotSupportedException();
    }

    public override int GetDataLength() => _modsBytesLength;

    protected override Stream GetDataStream() => _stream;
}