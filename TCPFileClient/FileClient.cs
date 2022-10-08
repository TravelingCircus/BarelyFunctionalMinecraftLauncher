using System.Collections.Concurrent;
using System.Net.Sockets;
using CommonData.Models;
using CommonData.Network;
using CommonData.Network.Messages.LaunchConfiguration;
using CommonData.Network.Messages.Login;
using CommonData.Network.Messages.Registration;
using CommonData.Network.Messages.Skin;
using CommonData.Network.Messages.Version;

namespace TCPFileClient;

public sealed class FileClient
{
    private string _minecraftPath;
    private TcpClient _client;
    private NetworkStream _networkStream;
    private NetworkChannel _networkChannel;
    private readonly ConcurrentQueue<Query> _requests;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public FileClient(string minecraftPath)
    {
        _minecraftPath = minecraftPath;
        _requests = new ConcurrentQueue<Query>();
        _cancellationTokenSource = new CancellationTokenSource();
    }

    #region Interface

    public Task<string> DownloadForgeFiles()
    {
        throw new NotImplementedException();
    }

    public async Task<LaunchConfiguration> DownloadLaunchConfiguration()
    {
        Message response = await GetResponseFor(new LaunchConfigurationRequest());
        return ((LaunchConfigurationResponse)response).LaunchConfiguration;
    }
    
    public async Task<ConfigurationVersion> DownloadConfigVersion()
    {
        Message response = await GetResponseFor(new ConfigVersionRequest());
        return ((ConfigVersionResponse)response).ConfigurationVersion;
    }

    public async Task<RegistrationResponse> SendRegistrationRequest(User user)
    {
        Message response = await GetResponseFor(new RegistrationRequest(user.Nickname, user.PasswordHash));
        return (RegistrationResponse)response;
    }
    
    public async Task<LoginResponse> SendLoginRequest(User user)
    {
        LoginResponse response = (LoginResponse)await GetResponseFor(new LoginRequest(user.Nickname, user.PasswordHash));
        
        string skinPath = _minecraftPath + @"\BFML\skin.png";
        await using FileStream stream = File.Open(skinPath, FileMode.OpenOrCreate);
        stream.Write(response.SkinData, 0, response.SkinData.Length);
        response.User.SkinPath = skinPath;

        return response;
    }

    public async Task DownloadMods(string directory)
    {
        throw new NotImplementedException();
    }
    
    public async Task<SkinChangeResponse> SendSkinChangeRequest(string nickname, string filePath)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(filePath)!);
        int bytesLength = (int)directoryInfo.GetFiles(Path.GetFileName(filePath))[0].Length;

        byte[] bytes = new byte[bytesLength];

        await using FileStream fileStream = new FileStream(filePath, FileMode.Open);
        _ = fileStream.Read(bytes, 0, bytesLength);
        
        Message response = await GetResponseFor(new SkinChangeRequest(nickname, bytes, bytesLength));
        return (SkinChangeResponse)response;
    }

    #endregion
    
    public bool ConnectToServer()
    {
        try
        {
            _client = new TcpClient("127.0.0.1", 69);
            _networkStream = _client.GetStream();
            _networkChannel = new NetworkChannel(_networkStream);
            CancellationToken cancellationToken = _cancellationTokenSource.Token;
            Task.Run(() =>
            {
                SendRequestsWhenAvailable(_networkChannel, _requests, cancellationToken).GetAwaiter();
            }, cancellationToken);
        }
        catch (Exception e)
        {
            return false;
        }

        return true;
    }

    private async Task SendRequestsWhenAvailable(NetworkChannel networkChannel, 
        ConcurrentQueue<Query> queryQueue, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (queryQueue.TryDequeue(out Query query))
            {
                await networkChannel.SendMessage(query.Request);
                MessageHeader header = await _networkChannel.ListenForHeader();
                Stream messageData = await _networkChannel.ListenForMessage(header);
                Message response = MessageRegistry.GetMessageFor(header);
                response.ApplyData(messageData);
                query.Response.SetResult(response);
            }
        }
    }
    
    private Task<Message> GetResponseFor(Message request)
    {
        TaskCompletionSource<Message> taskCompletionSource = new TaskCompletionSource<Message>();
        _requests.Enqueue(new Query(request, taskCompletionSource));
        return taskCompletionSource.Task;
    }

    private void Terminate()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _networkStream.Dispose();
        _client.Close();
    }
    
    private class Query
    {
        public readonly Message Request;
        public readonly TaskCompletionSource<Message> Response;

        public Query(Message request, TaskCompletionSource<Message> response)
        {
            Request = request;
            Response = response;
        }
    }
}