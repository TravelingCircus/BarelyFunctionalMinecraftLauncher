namespace Common.Network.Messages;

public class ExterminatusRequest: Message
{
    public override MessageHeader GetHeader()
    {
        return new MessageHeader(MessageRegistry.GetKeyForMessageType(typeof(ExterminatusRequest)), 0);
    }

    public override void ApplyData(Stream stream)
    {
        throw new NotSupportedException();
    }

    protected override Stream GetData() => new MemoryStream();
}