using System.Net;
using System.Net.Sockets;
using Common.Network;
using Common.Network.Messages.ChangeSkin;
using Common.Network.Messages.ForgeDownload;
using Common.Network.Messages.LaunchConfiguration;
using Common.Network.Messages.Login;
using Common.Network.Messages.ModsDownload;
using Common.Network.Messages.Registration;
using Common.Network.Messages.Version;
using TCPFileServer.DataAccess;
using TCPFileServer.MessageHandlers;

namespace TCPFileServer;

public sealed class Server
{
    public bool IsRunning { get; private set; }
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly TcpListener _tcpListener;
    
    public Server()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _tcpListener = new TcpListener(IPAddress.Any, 69);
    }

    public void Start()
    {
        CancellationToken cancellationToken = _cancellationTokenSource.Token;
        
        Repository repository = new Repository(@"C:\Users\maksy\Desktope\TestRepo\", cancellationToken);
        //Repository repository = new Repository(@"D:\Home\Desktope\TestDownloads\", cancellationToken);
        repository.Initialize();
        HandlerPicker.RegisterHandler(nameof(RegistrationRequest), new RegistrationHandler(repository));
        HandlerPicker.RegisterHandler(nameof(LoginRequest), new LoginHandler(repository));
        HandlerPicker.RegisterHandler(nameof(LaunchConfigurationRequest), new LaunchConfigurationHandler(repository));
        HandlerPicker.RegisterHandler(nameof(ConfigVersionRequest), new ConfigVersionHandler(repository));
        HandlerPicker.RegisterHandler(nameof(SkinChangeRequest), new SkinChangeHandler(repository));
        HandlerPicker.RegisterHandler(nameof(ForgeDownloadRequest), new ForgeDownloadHandler(repository));
        HandlerPicker.RegisterHandler(nameof(ModsDownloadRequest), new ModsDownloadHandler(repository));
        //TODO properly register handlers
        
        _tcpListener.Start();
        IsRunning = true;
        Console.WriteLine($"STARTED thread_{Thread.CurrentThread.ManagedThreadId}");
        Task.Run(() =>
        {
            AcceptClientsBlocking(cancellationToken);
        }, cancellationToken);
    }

    private void AcceptClientsBlocking(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine($"LISTENING thread_{Thread.CurrentThread.ManagedThreadId}");
            TcpClient tcpClient = _tcpListener.AcceptTcpClient();
            if(tcpClient.Connected)Console.WriteLine($"RECEIVED CONNECTION thread_{Thread.CurrentThread.ManagedThreadId}");
            Task.Run(() =>
            {
                HandleClient(tcpClient).GetAwaiter();
            }, cancellationToken);
        }
        Console.WriteLine($"STOPPED LISTENING thread_{Thread.CurrentThread.ManagedThreadId}");
    }
    
    private async Task HandleClient(TcpClient client)
    {
        NetworkStream networkStream = client.GetStream();
        NetworkChannel networkChannel = new NetworkChannel(networkStream);
        while (client.Connected)
        {
            MessageHeader header = await networkChannel.ListenForHeader();
            Stream messageData = await networkChannel.ListenForMessage(header);
            MessageHandler messageHandler = HandlerPicker.GetHandler(header);
            Message response = await messageHandler.GetResponse(messageData);
            await networkChannel.SendMessage(response);
            Console.WriteLine($"SENT RESPONSE thread_{Thread.CurrentThread.ManagedThreadId}");
        }
        
        Console.WriteLine($"HANDLING CONNECTION thread_{Thread.CurrentThread.ManagedThreadId}");
    }

    public void Terminate()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _tcpListener.Stop();
        IsRunning = false;
        Console.WriteLine($"TERMINATED thread_{Thread.CurrentThread.ManagedThreadId}");
    }
}