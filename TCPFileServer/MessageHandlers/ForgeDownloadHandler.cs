using CommonData.Network;
using CommonData.Network.Messages;
using TCPFileServer.DataAccess;

namespace TCPFileServer.MessageHandlers;

public class ForgeDownloadHandler: MessageHandler
{
    private Repository _repository;

    public ForgeDownloadHandler(Repository repository)
    {
        _repository = repository;
    }

    public override Task Handle(Stream dataStream)
    {
        throw new NotSupportedException();
    }

    public override async Task<Message> GetResponse(Stream dataStream)
    {
        Console.WriteLine($"HANDLING FORGE DOWNLOAD REQUEST thread_{Thread.CurrentThread.ManagedThreadId}");
        Stream stream = await _repository.GetForgeArchiveStream();
        ForgeDownloadResponse response = new ForgeDownloadResponse(stream, (int)stream.Length);
        Console.WriteLine($"HANDLED REQUEST thread_{Thread.CurrentThread.ManagedThreadId}");
        return response;
    }
}