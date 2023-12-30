using Common.Network;
using Common.Network.Messages.GetSkin;
using TCPFileServer.DataAccess;

namespace TCPFileServer.MessageHandlers;

public sealed class GetSkinHandler: MessageHandler
{
    public GetSkinHandler(Repository repository) : base(repository) { }

    public override Task Handle(Stream dataStream)
    {
        throw new NotSupportedException();
    }

    public override async Task<Message> GetResponse(Stream dataStream)
    {
        Console.WriteLine($"HANDLING GET SKIN REQUEST thread_{Thread.CurrentThread.ManagedThreadId}");
        GetSkinRequest request = new GetSkinRequest();
        request.ApplyData(dataStream);
        byte[] skinBytes = await Repository.GetSkin(request.Nickname);
        GetSkinResponse response = new GetSkinResponse(skinBytes.Length, skinBytes);
        Console.WriteLine($"SENT SKIN thread_{Thread.CurrentThread.ManagedThreadId}");
        return response;
    }
}