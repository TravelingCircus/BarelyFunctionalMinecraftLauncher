namespace CommonData.Network.Messages;

public class RegistrationResponse : Message
{
    public bool Success;

    public RegistrationResponse(bool success)
    {
        Success = success;
    }

    public override MessageHeader GetHeader()
    {
        throw new NotImplementedException();
    }

    public override void FromData(Stream stream)
    {
        throw new NotImplementedException();
    }

    protected override Stream GetData()
    {
        throw new NotImplementedException();
    }
}