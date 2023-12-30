namespace Common.Network.Messages.Registration;

public class RegistrationResponse : Message
{
    public bool Success;

    public RegistrationResponse() { }
    
    public RegistrationResponse(bool success)
    {
        Success = success;
    }

    public override MessageHeader GetHeader()
    {
        return new MessageHeader(MessageRegistry.GetKeyForMessageType(typeof(RegistrationResponse)), 1);
    }

    public override void ApplyData(Stream stream)
    {
        Success = BoolReadStream(stream);
    }

    protected override Stream GetData()
    {
        MemoryStream buffer = new MemoryStream(1);
        byte[] successBytes = BitConverter.GetBytes(Success);
        buffer.Write(successBytes);
        return buffer;
    }
}