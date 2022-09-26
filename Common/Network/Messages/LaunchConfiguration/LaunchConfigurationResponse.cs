namespace CommonData.Network.Messages.LaunchConfiguration;

public class LaunchConfigurationResponse: Message
{
    public Models.LaunchConfiguration LaunchConfiguration;

    public LaunchConfigurationResponse()
    {
    }

    public LaunchConfigurationResponse(Models.LaunchConfiguration launchConfiguration)
    {
        LaunchConfiguration = launchConfiguration;
    }

    public override MessageHeader GetHeader()
    {
        return new MessageHeader(MessageRegistry.GetKeyForMessageType(typeof(LaunchConfigurationResponse)), 
            LaunchConfiguration.ForgeVersion.Length + 
            LaunchConfiguration.ModsChecksum.Length + 
            LaunchConfiguration.VanillaVersion.Length);    
    }

    public override void ApplyData(Stream stream)
    {
        LaunchConfiguration = new Models.LaunchConfiguration(
            StringReadStream(stream),
            StringReadStream(stream),
            StringReadStream(stream));
    }

    protected override Stream GetData()
    {
        MemoryStream buffer = new MemoryStream(GetHeader().DataLength);
        WriteToStream(buffer, LaunchConfiguration.VanillaVersion);
        WriteToStream(buffer, LaunchConfiguration.ForgeVersion);
        WriteToStream(buffer, LaunchConfiguration.ModsChecksum);
        
        buffer.Flush();
        buffer.Position = 0;
        return buffer;
    }
}