using CommonData;
using CommonData.Models;
using CommonData.Network;
using CommonData.Network.Messages;
using HTTPFileServer.DataAccess;

namespace HTTPFileServer.MessageHandlers;

public class LoginHandler: MessageHandler
{
    private Repository _repository;

    public LoginHandler(Repository repository)
    {
        _repository = repository;
    }

    public override bool CanHandle(MessageHeader messageHeader)
    {
        return MessageRegistry.GetMessageTypeName(messageHeader) == nameof(LoginRequest);
    }

    public override Task Handle(Stream dataStream)
    {
        throw new NotSupportedException();
    }

    public override async Task<Message> GetResponse(Stream dataStream)
    {
        Console.WriteLine($"HANDLING LOGIN REQUEST thread_{Thread.CurrentThread.ManagedThreadId}");
        LoginRequest request = new LoginRequest();
        request.ApplyData(dataStream);
        bool success = await _repository.TryLogIn(request.NickName, request.PasswordHash);
        Console.WriteLine($"SENT RESPONSE:{success} thread_{Thread.CurrentThread.ManagedThreadId}");
        return new LoginResponse(success);
    }
}