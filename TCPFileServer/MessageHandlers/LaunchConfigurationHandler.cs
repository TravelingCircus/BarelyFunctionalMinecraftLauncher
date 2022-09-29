using CommonData.Models;
using CommonData.Network;
using CommonData.Network.Messages.LaunchConfiguration;
using TCPFileServer.DataAccess;

namespace TCPFileServer.MessageHandlers;

public class LaunchConfigurationHandler: MessageHandler
{
    private Repository _repository;

    public LaunchConfigurationHandler(Repository repository)
    {
        _repository = repository;
    }

    public override Task Handle(Stream dataStream)
    {
        throw new InvalidOperationException();
    }

    public override async Task<Message> GetResponse(Stream dataStream)
    {
        Console.WriteLine($"HANDLING LAUNCH CONFIGURATION REQUEST thread_{Thread.CurrentThread.ManagedThreadId}");
        LaunchConfiguration launchConfiguration = await _repository.GetLaunchConfiguration();
        return new LaunchConfigurationResponse(launchConfiguration);
    }
}