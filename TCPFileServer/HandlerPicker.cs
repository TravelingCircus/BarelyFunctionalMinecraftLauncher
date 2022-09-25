namespace CommonData.Network;

public static class HandlerPicker
{
    private static readonly Dictionary<string, MessageHandler> _keyToHandler = new Dictionary<string, MessageHandler>();

    
    public static MessageHandler GetHandler(MessageHeader messageHeader)
    {
        string messageType = MessageRegistry.GetMessageTypeName(messageHeader);
        if (!_keyToHandler.ContainsKey(messageType))
        {
            throw new ArgumentOutOfRangeException(nameof(messageHeader),
                $"No handler registered for key: {messageHeader.MessageKey}");
        }
        return _keyToHandler[messageType];
    }

    public static void RegisterHandler(string messageType, MessageHandler handler)
    {
        _keyToHandler.Add(messageType, handler);
    }
}