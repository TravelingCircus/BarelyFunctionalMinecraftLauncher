namespace CommonData.Network.Messages;

public sealed class ForgeDownloadRequest : Message
{
    public string FilePath;
    
    public ForgeDownloadRequest()
    {
        
    }
    
    public ForgeDownloadRequest(string filePath)
    {
        FilePath = filePath;
    }
    
    public override MessageHeader GetHeader()
    {
        return new MessageHeader(MessageRegistry.GetKeyForMessageType(typeof(ForgeDownloadRequest)),
            GetFileSizeBytes(FilePath));
    }

    public override void ApplyData(Stream stream)
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