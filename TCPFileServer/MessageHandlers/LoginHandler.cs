using Common.Models;
using Common.Network;
using Common.Network.Messages.Login;
using TCPFileServer.DataAccess;

namespace TCPFileServer.MessageHandlers;

public class LoginHandler: MessageHandler
{
    public LoginHandler(Repository repository): base(repository) 
    {
        
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
        bool success = await Repository.TryLogIn(request.NickName, request.PasswordHash);
        User user = success ? await Repository.GetUser(request.NickName) : new User();
        byte[] skinData = success ? await Repository.GetSkin(request.NickName) : Array.Empty<byte>();
        return new LoginResponse(success, user, skinData);
    }
}