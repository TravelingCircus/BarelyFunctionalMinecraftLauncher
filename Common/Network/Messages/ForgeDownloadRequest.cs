namespace CommonData.Network.Messages;

public sealed class ForgeDownloadRequest : Message
{
    public const byte Key = 5;
    public string FilePath;
    
    public ForgeDownloadRequest(string filePath)
    {
        FilePath = filePath;
    }
    
    public override MessageHeader GetHeader()
    {
        return new MessageHeader(Key,GetFileSizeBytes(FilePath));
    }

    public override void FromData(Stream stream)
    {
        throw new NotImplementedException();
    }

    protected override Stream GetData()
    {
        throw new NotImplementedException();
    }

    private int GetFileSizeBytes(string path)
    {
        long size = new FileInfo(path).Length;
        if (size > Int32.MaxValue) throw new OverflowException("Can't send files larger than 2GB");
        return (int)size;
    }
}