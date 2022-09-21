using CommonData.Network;
using HTTPFileServer.MessageHandlers;

namespace HTTPFileServer;

public class HandlerPicker
{
    private static Dictionary<byte, MessageHandler> _keyToHandler = new Dictionary<byte, MessageHandler>()
    {
        {0, new RegistrationHandler()}
    };

    public static MessageHandler GetFor(Message message)
    {
        byte messageKey = message.GetHeader()[0];
        return _keyToHandler[messageKey];
    }
}