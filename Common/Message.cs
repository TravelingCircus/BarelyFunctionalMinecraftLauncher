namespace CommonData;

public abstract class Message
{
    protected byte[] messageBytes;

    public byte[] MessageToByteArray()
    {
        return DataConverter.ObjectToByteArray(this);
    }
    
    public Message ByteArrayToMessage()
    {
        return DataConverter.ByteArrayToObject<Message>(messageBytes);
    }
}