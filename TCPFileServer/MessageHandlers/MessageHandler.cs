using Common.Network;
using TCPFileServer.DataAccess;

namespace TCPFileServer.MessageHandlers;

public abstract class MessageHandler
{
    protected readonly Repository Repository;
    
    protected MessageHandler(Repository repository)
    {
        Repository = repository;
    }
    
    public bool CanHandle(MessageHeader messageHeader)
    {
        return MessageRegistry.GetMessageTypeName(messageHeader) == GetType().Name;
    }
    
    public abstract Task Handle(Stream dataStream);
    public abstract Task<Message> GetResponse(Stream dataStream);
}