using CommonData.Network;

namespace HTTPFileServer.MessageHandlers;

public class RegistrationHandler : MessageHandler
{
    public override bool CanHandle(Message message)
    {
        throw new NotImplementedException();
    }

    public override Task Handle(Stream dataStream)
    {
        throw new NotImplementedException();
    }

    public override Task<Message> GetResponse(Stream dataStream)
    {
        throw new NotImplementedException();
    }
}