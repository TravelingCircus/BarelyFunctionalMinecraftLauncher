namespace CommonData.Network;

public class MessageHeader
{
    public readonly byte MessageKey;
    public readonly int DataLength;

    public MessageHeader(byte messageKey, int dataLength)
    {
        MessageKey = messageKey;
        DataLength = dataLength;
    }
}