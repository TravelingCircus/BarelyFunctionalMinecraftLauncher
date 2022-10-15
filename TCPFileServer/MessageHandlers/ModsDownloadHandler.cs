using Common.Network;
using Common.Network.Messages.ModsDownload;
using TCPFileServer.DataAccess;

namespace TCPFileServer.MessageHandlers;

public sealed class ModsDownloadHandler: MessageHandler
{
    public ModsDownloadHandler(Repository repository) : base(repository)
    {
        
    }

    public override Task Handle(Stream dataStream)
    {
        throw new NotSupportedException();
    }

    public override async Task<Message> GetResponse(Stream dataStream)
    {
        Console.WriteLine($"HANDLING MODS DOWNLOAD REQUEST thread_{Thread.CurrentThread.ManagedThreadId}");
        BorrowableFileStream stream = await Repository.GetModsArchiveStream();
        ModsDownloadResponse response = new ModsDownloadResponse(stream, (int)new FileInfo(stream.GetFileName()).Length);
        Console.WriteLine($"HANDLED REQUEST thread_{Thread.CurrentThread.ManagedThreadId}");
        return response;
    }
}