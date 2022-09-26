namespace CommonData.Network;

public abstract class MessageHandler
{
    public bool CanHandle(MessageHeader messageHeader)
    {
        return MessageRegistry.GetMessageTypeName(messageHeader) == GetType().Name;
    }
    
    public abstract Task Handle(Stream dataStream);
    public abstract Task<Message> GetResponse(Stream dataStream);
}