using CommonData;
using CommonData.Network;
using CommonData.Network.Messages;
using HTTPFileServer.DataAccess;

namespace HTTPFileServer.MessageHandlers;

public sealed class RegistrationHandler : MessageHandler
{
    private Repository _repository;

    public RegistrationHandler(Repository repository)
    {
        _repository = repository;
    }

    public override bool CanHandle(MessageHeader messageHeader)
    {
        return messageHeader.MessageKey == 1;
    }

    public override Task Handle(Stream dataStream)
    {
        throw new NotImplementedException();
    }

    public override async Task<Message> GetResponse(Stream dataStream)
    {
        RegistrationRequest request = new RegistrationRequest();
        request.FromData(dataStream);
        bool success = await _repository.AddNewUser(new User(request.NickName, request.PasswordHash, 0));
        return new RegistrationResponse(success);
    }
}