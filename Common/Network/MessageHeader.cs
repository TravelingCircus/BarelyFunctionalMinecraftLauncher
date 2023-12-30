namespace Common.Network;

public sealed class MessageHeader
{
    public readonly byte MessageKey;
    public readonly int DataLength;

    public MessageHeader(byte messageKey, int dataLength)
    {
        MessageKey = messageKey;
        DataLength = dataLength;
    }

    public byte[] ToByteArray()
    {
        byte[] bytes = new byte[5];
        bytes[0] = MessageKey;
        byte[] dataLengthBytes = BitConverter.GetBytes(DataLength);
        for (int i = 0; i < 4; i++)
        {
            bytes[i + 1] = dataLengthBytes[i];
        }
        return bytes;
    }
}