namespace Common.Network.Messages.ForgeDownload;

public sealed class ForgeDownloadRequest : Message
{
    public override MessageHeader GetHeader()
    {
        return new MessageHeader(MessageRegistry.GetKeyForMessageType(typeof(ForgeDownloadRequest)), 0);
    }

    public override void ApplyData(Stream stream)
    {
        throw new NotSupportedException();
    }

    protected override Stream GetData() => new MemoryStream();
}