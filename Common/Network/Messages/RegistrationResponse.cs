namespace CommonData.Network.Messages;

public class RegistrationResponse : Message
{
    public bool Success;
    public const byte Key = 2;

    public RegistrationResponse(bool success)
    {
        Success = success;
    }

    public override MessageHeader GetHeader()
    {
        return new MessageHeader(Key, 1);
    }

    public override void FromData(Stream stream)
    {
        Success = BoolReadStream(stream);
    }

    protected override Stream GetData()
    {
        MemoryStream buffer = new MemoryStream();
        byte[] successBytes = BitConverter.GetBytes(Success);
        buffer.Write(successBytes);
        return buffer;
    }
}