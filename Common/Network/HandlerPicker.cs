using CommonData.Network;

namespace HTTPFileServer;

public class HandlerPicker
{
    private static readonly Dictionary<byte, MessageHandler> _keyToHandler = new Dictionary<byte, MessageHandler>()
    {
        //{0, new RegistrationHandler()}
    };

    public static MessageHandler GetHandler(MessageHeader messageHeader)
    {
        return _keyToHandler[messageHeader.MessageKey];
    }
    
    
}