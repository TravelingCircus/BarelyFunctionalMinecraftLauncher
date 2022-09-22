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

    public static MessageHeader GetFromMessage(Message message)
    {
        byte[] messageBytes = message.GetHeader();
        int dataLength = BitConverter.ToInt32(messageBytes, 1);
        return new MessageHeader(messageBytes[0], dataLength);
    }
}