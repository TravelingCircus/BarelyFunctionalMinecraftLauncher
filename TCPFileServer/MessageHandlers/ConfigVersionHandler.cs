using Common.Models;
using Common.Network;
using Common.Network.Messages.Version;
using TCPFileServer.DataAccess;

namespace TCPFileServer.MessageHandlers;

public class ConfigVersionHandler : MessageHandler
{
    public ConfigVersionHandler(Repository repository): base(repository) { }

    public override Task Handle(Stream dataStream)
    {
        throw new InvalidOperationException();
    }

    public override async Task<Message> GetResponse(Stream dataStream)
    {
        Console.WriteLine($"HANDLING CONFIG VERSION REQUEST thread_{Thread.CurrentThread.ManagedThreadId}");
        ConfigurationVersion configVersion = await Repository.GetConfigurationVersion();
        return new ConfigVersionResponse(configVersion);
    }
}