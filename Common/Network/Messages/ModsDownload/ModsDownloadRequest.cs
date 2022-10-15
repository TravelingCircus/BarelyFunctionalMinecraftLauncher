namespace Common.Network.Messages.ModsDownload;

public sealed class ModsDownloadRequest : Message
{
    public override MessageHeader GetHeader()
    {
        return new MessageHeader(MessageRegistry.GetKeyForMessageType(typeof(ModsDownloadRequest)), 0);
    }

    public override void ApplyData(Stream stream)
    {
        throw new NotSupportedException();
    }

    protected override Stream GetData()
    {
        return new MemoryStream();
    }
}