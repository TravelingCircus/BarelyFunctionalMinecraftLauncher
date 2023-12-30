namespace Common.Network.Messages.LaunchConfiguration;

public class LaunchConfigurationRequest: Message
{
    public override MessageHeader GetHeader()
    {
        return new MessageHeader(MessageRegistry.GetKeyForMessageType(typeof(LaunchConfigurationRequest)), 0);
    }

    public override void ApplyData(Stream stream)
    {
        throw new NotSupportedException();
    }

    protected override Stream GetData() => new MemoryStream();
}