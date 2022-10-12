using Common.Models;
using Common.Network;
using Common.Network.Messages.Registration;
using TCPFileServer.DataAccess;

namespace TCPFileServer.MessageHandlers;

public sealed class RegistrationHandler : MessageHandler
{
    public RegistrationHandler(Repository repository): base(repository) 
    {
        
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
        bool success = await Repository.TryRegisterUser(new User(request.NickName, request.PasswordHash));
        Console.WriteLine($"SENT RESPONSE:{success} thread_{Thread.CurrentThread.ManagedThreadId}");
        return new RegistrationResponse(success);
    }
}