using CommonData.Network;

namespace HTTPFileServer;

public class HandlePicker
{
    private static Dictionary<byte, MessageHandler> _keyToHandler = new Dictionary<byte, MessageHandler>();

    public static MessageHandler GetFor(Message message)
    {
        message.GetHeader();
    }
}