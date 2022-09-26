using CommonData;
using CommonData.Models;
using CommonData.Network;
using CommonData.Network.Messages;
using CommonData.Network.Messages.Registration;
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
        return MessageRegistry.GetMessageTypeName(messageHeader) == nameof(RegistrationRequest);
    }

    public override Task Handle(Stream dataStream)
    {
        throw new NotSupportedException();
    }

    public override async Task<Message> GetResponse(Stream dataStream)
    {
        Console.WriteLine($"HANDLING REGISTRATION REQUEST thread_{Thread.CurrentThread.ManagedThreadId}");
        RegistrationRequest request = new RegistrationRequest();
        request.ApplyData(dataStream);
        bool success = await _repository.TryRegisterUser(new User(request.NickName, request.PasswordHash));
        Console.WriteLine($"SENT RESPONSE:{success} thread_{Thread.CurrentThread.ManagedThreadId}");
        return new RegistrationResponse(success);
    }
}