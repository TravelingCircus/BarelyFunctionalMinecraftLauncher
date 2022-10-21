namespace Common.Network.Messages.LaunchConfiguration;

public class LaunchConfigurationResponse: Message
{
    public Common.Models.LaunchConfiguration LaunchConfiguration;

    public LaunchConfigurationResponse()
    {
    }

    public LaunchConfigurationResponse(Common.Models.LaunchConfiguration launchConfiguration)
    {
        LaunchConfiguration = launchConfiguration;
    }

    public override MessageHeader GetHeader()
    {
        return new MessageHeader(MessageRegistry.GetKeyForMessageType(typeof(LaunchConfigurationResponse)), 
            LaunchConfiguration.ForgeVersion.Length + 
            LaunchConfiguration.ModsChecksum.Length + 
            LaunchConfiguration.VanillaVersion.Length + sizeof(int) * 4);    
    }

    public override void ApplyData(Stream stream)
    {
        LaunchConfiguration = new Common.Models.LaunchConfiguration(
            StringReadStream(stream),
            StringReadStream(stream),
            StringReadStream(stream),
            IntReadStream(stream));
    }

    protected override Stream GetData()
    {
        MemoryStream buffer = new MemoryStream(GetHeader().DataLength);
        WriteToStream(buffer, LaunchConfiguration.VanillaVersion);
        WriteToStream(buffer, LaunchConfiguration.ForgeVersion);
        WriteToStream(buffer, LaunchConfiguration.ModsChecksum);
        WriteToStream(buffer, LaunchConfiguration.RequiredGriwnas);
        
        buffer.Flush();
        buffer.Position = 0;
        return buffer;
    }
}