using CommonData.Network;
using CommonData.Network.Messages.Login;
using HTTPFileServer.DataAccess;

namespace HTTPFileServer.MessageHandlers;

public class LoginHandler: MessageHandler
{
    private Repository _repository;

    public LoginHandler(Repository repository)
    {
        _repository = repository;
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
        return new LoginResponse(success);
    }
}