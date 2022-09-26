namespace CommonData.Network.Messages.Version;

public class ConfigVersionRequest : Message
{
    public override MessageHeader GetHeader()
    {
        return new MessageHeader(MessageRegistry.GetKeyForMessageType(typeof(ConfigVersionRequest)), 0);
    }

    public override void ApplyData(Stream stream)
    {
        throw new NotImplementedException();
    }

    protected override Stream GetData()
    {
        return new MemoryStream(0);
    }
}