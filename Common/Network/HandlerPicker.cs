namespace CommonData.Network;

public static class HandlerPicker
{
    private static readonly Dictionary<byte, MessageHandler> _keyToHandler = new Dictionary<byte, MessageHandler>();

    
    public static MessageHandler GetHandler(MessageHeader messageHeader)
    {
        if (_keyToHandler.ContainsKey(messageHeader.MessageKey))
        {
            throw new ArgumentOutOfRangeException(nameof(messageHeader),
                $"No handler registered for key: {messageHeader.MessageKey}");
        }
        return _keyToHandler[messageHeader.MessageKey];
    }

    public static void RegisterHandler(byte key, MessageHandler handler)
    {
        _keyToHandler.Add(key, handler);
    }
}