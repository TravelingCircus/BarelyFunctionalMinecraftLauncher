using System.Net;
using System.Net.Sockets;
using Common.Network;
using Common.Network.Messages.ChangeSkin;
using Common.Network.Messages.ForgeDownload;
using Common.Network.Messages.GetSkin;
using Common.Network.Messages.Login;
using Common.Network.Messages.ModsDownload;
using Common.Network.Messages.Registration;
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

        string projectFilesPath = Environment.CurrentDirectory+@"/Repo/";
        Repository repository = new Repository(projectFilesPath, cancellationToken);
        repository.Initialize();
        RegisterHandlers(repository);

        _tcpListener.Start();
        IsRunning = true;
        Task.Run(() => AcceptClientsBlocking(cancellationToken), cancellationToken);
    }

    public void Terminate()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _tcpListener.Stop();
        IsRunning = false;
    }

    private void AcceptClientsBlocking(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            TcpClient tcpClient = _tcpListener.AcceptTcpClient();
            Task.Run(() =>
                {
                    try
                    {
                        HandleClient(tcpClient).GetAwaiter();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                },
                cancellationToken);
        }
    }

    private static async Task HandleClient(TcpClient client)
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
        }
    }

    private static void RegisterHandlers(Repository repository)
    {
        HandlerPicker.RegisterHandler(nameof(RegistrationRequest), new RegistrationHandler(repository));
        HandlerPicker.RegisterHandler(nameof(LoginRequest), new LoginHandler(repository));
        HandlerPicker.RegisterHandler(nameof(SkinChangeRequest), new SkinChangeHandler(repository));
        HandlerPicker.RegisterHandler(nameof(ForgeDownloadRequest), new ForgeDownloadHandler(repository));
        HandlerPicker.RegisterHandler(nameof(ModsDownloadRequest), new ModsDownloadHandler(repository));
        HandlerPicker.RegisterHandler(nameof(GetSkinRequest), new GetSkinHandler(repository));
    }
}