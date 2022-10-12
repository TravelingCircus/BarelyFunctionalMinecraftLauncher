using Common.Network;
using Common.Network.Messages.ForgeDownload;
using CommonData.Network;
using CommonData.Network.Messages;
using TCPFileServer.DataAccess;

namespace TCPFileServer.MessageHandlers;

public class ForgeDownloadHandler: MessageHandler
{
    public ForgeDownloadHandler(Repository repository) : base(repository) 
    {
         
    }
    
    public override Task Handle(Stream dataStream)
    {
        throw new NotSupportedException();
    }

    public override async Task<Message> GetResponse(Stream dataStream)
    {
        Console.WriteLine($"HANDLING FORGE DOWNLOAD REQUEST thread_{Thread.CurrentThread.ManagedThreadId}");
        BorrowableFileStream stream = await Repository.GetForgeArchiveStream();
        ForgeDownloadResponse response = new ForgeDownloadResponse(stream, (int)new FileInfo(stream.GetFileName()).Length);
        Console.WriteLine($"HANDLED REQUEST thread_{Thread.CurrentThread.ManagedThreadId}");
        return response;
    }
}