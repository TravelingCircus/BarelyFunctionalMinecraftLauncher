namespace CommonData.Network;

public abstract class MessageHandler
{
    public abstract bool CanHandle(MessageHeader messageHeader);
    public abstract Task Handle(Stream dataStream);
    public abstract Task<Message> GetResponse(Stream dataStream);
}