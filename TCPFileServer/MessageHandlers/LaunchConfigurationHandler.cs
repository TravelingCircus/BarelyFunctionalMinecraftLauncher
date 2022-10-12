using Common.Models;
using Common.Network;
using Common.Network.Messages.LaunchConfiguration;
using TCPFileServer.DataAccess;

namespace TCPFileServer.MessageHandlers;

public class LaunchConfigurationHandler: MessageHandler
{
    public LaunchConfigurationHandler(Repository repository): base(repository) 
    {
        
    }

    public override Task Handle(Stream dataStream)
    {
        throw new InvalidOperationException();
    }

    public override async Task<Message> GetResponse(Stream dataStream)
    {
        Console.WriteLine($"HANDLING LAUNCH CONFIGURATION REQUEST thread_{Thread.CurrentThread.ManagedThreadId}");
        LaunchConfiguration launchConfiguration = await Repository.GetLaunchConfiguration();
        return new LaunchConfigurationResponse(launchConfiguration);
    }
}