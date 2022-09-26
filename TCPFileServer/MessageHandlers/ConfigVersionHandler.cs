using CommonData.Models;
using CommonData.Network;
using CommonData.Network.Messages.Version;
using TCPFileServer.DataAccess;

namespace TCPFileServer.MessageHandlers;

public class ConfigVersionHandler : MessageHandler
{
    private Repository _repository;

    public ConfigVersionHandler(Repository repository)
    {
        _repository = repository;
    }

    public override Task Handle(Stream dataStream)
    {
        throw new NotImplementedException();
    }

    public override async Task<Message> GetResponse(Stream dataStream)
    {
        Console.WriteLine($"HANDLING CONFIG VERSION REQUEST thread_{Thread.CurrentThread.ManagedThreadId}");
        ConfigurationVersion configVersion = await _repository.GetConfigurationVersion();
        return new ConfigVersionResponse(configVersion);
    }
}