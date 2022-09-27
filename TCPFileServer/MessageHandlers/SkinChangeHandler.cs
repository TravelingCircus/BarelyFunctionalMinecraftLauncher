using CommonData.Network;
using CommonData.Network.Messages.Skin;
using TCPFileServer.DataAccess;

namespace TCPFileServer.MessageHandlers;

public class SkinChangeHandler: MessageHandler
{
    private Repository _repository;

    public SkinChangeHandler(Repository repository)
    {
        _repository = repository;
    }
    
    public override Task Handle(Stream dataStream)
    {
        throw new NotSupportedException();
    }

    public override async Task<Message> GetResponse(Stream dataStream)
    {
        Console.WriteLine($"HANDLING SKIN CHANGE REQUEST thread_{Thread.CurrentThread.ManagedThreadId}");
        SkinChangeRequest request = new SkinChangeRequest();
        request.ApplyData(dataStream);
        bool success = await _repository.UpdateUserSkin(request.Nickname, request.SkinData);
        Console.WriteLine($"SENT RESPONSE:{success} thread_{Thread.CurrentThread.ManagedThreadId}");
        return new SkinChangeResponse(success);
    }
}