using Common.Models;

namespace Common.Network.Messages.Version;

public class ConfigVersionResponse : Message
{
    public ConfigurationVersion ConfigurationVersion;

    public ConfigVersionResponse() { }

    public ConfigVersionResponse(ConfigurationVersion configVersion)
    {
        ConfigurationVersion = configVersion;
    }

    public override MessageHeader GetHeader()
    {
        return new MessageHeader(MessageRegistry.GetKeyForMessageType(typeof(ConfigVersionResponse)),
            3 * sizeof(int)
            + ConfigurationVersion.Changelog.Length);
    }

    public override void ApplyData(Stream stream)
    {
        ConfigurationVersion = new ConfigurationVersion(
            IntReadStream(stream),
            IntReadStream(stream),
            StringReadStream(stream));
    }

    protected override Stream GetData()
    {
        MemoryStream buffer = new MemoryStream(GetHeader().DataLength);
        WriteToStream(buffer, ConfigurationVersion.MajorVersion);
        WriteToStream(buffer, ConfigurationVersion.MinorVersion);
        WriteToStream(buffer, ConfigurationVersion.Changelog);
            
        buffer.Flush();
        buffer.Position = 0;
        return buffer;
    }
}