using System.Collections.Concurrent;
using System.Net.Sockets;
using Common;
using Common.Misc;
using Common.Models;
using Common.Network;
using Common.Network.Messages.ChangeSkin;
using Common.Network.Messages.ForgeDownload;
using Common.Network.Messages.GetSkin;
using Common.Network.Messages.LaunchConfiguration;
using Common.Network.Messages.Login;
using Common.Network.Messages.ModsDownload;
using Common.Network.Messages.Registration;
using Common.Network.Messages.Version;

namespace FileClient;

public sealed class ServerConnection : IFileClient
{
    private TcpClient _client;
    private NetworkStream _networkStream;
    private NetworkChannel _networkChannel;
    private readonly int _port;
    private readonly string _hostName;
    private readonly ConcurrentQueue<Query> _requests;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public ServerConnection(string hostName, int port)
    {
        _port = port;
        _hostName = hostName;
        _requests = new ConcurrentQueue<Query>();
        _cancellationTokenSource = new CancellationTokenSource();
    }

    #region Interface
    
    public Task<bool> TryInit()
    {
        bool result = ConnectToServer(_hostName, _port);
        return Task.FromResult(result);
    }

    public Task<bool> TryDispose()
    {
        Terminate();
        return Task.FromResult(true);
    }

    public async Task<Result<User>> Authenticate(User user)
    {
        LoginRequest request = new LoginRequest(user.Nickname, user.PasswordHash);
        LoginResponse response = (LoginResponse)await GetResponseFor(request);
        return response.Success 
            ? Result<User>.Ok(user)
            : Result<User>.Err(new ArgumentException($"Can't authenticate user {user.Nickname}."));
    }

    public async Task<Result<User>> CreateRecord(User user)
    {
        RegistrationRequest request = new RegistrationRequest(user.Nickname, user.PasswordHash);
        RegistrationResponse response = (RegistrationResponse)await GetResponseFor(request);
        return response.Success 
            ? Result<User>.Ok(user)
            : Result<User>.Err(new ArgumentException($"Can't create record for user {user.Nickname}."));
    }

    public async Task<LaunchConfiguration> LoadLaunchConfiguration()
    {
        Message response = await GetResponseFor(new LaunchConfigurationRequest());
        return ((LaunchConfigurationResponse)response).LaunchConfiguration;
    }

    public async Task<ConfigurationVersion> LoadConfigVersion()
    {
        Message response = await GetResponseFor(new ConfigVersionRequest());
        return ((ConfigVersionResponse)response).ConfigurationVersion;
    }
    
    public async Task<bool> TryLoadForge(FileInfo target)
    {
        ForgeDownloadRequest request = new ForgeDownloadRequest();
        ForgeDownloadResponse response = (ForgeDownloadResponse)await GetResponseFor(request);

        try
        {
            await using FileStream fileStream = target.Open(FileMode.OpenOrCreate);
            await fileStream.WriteAsync(response.ForgeBytes.AsMemory(0, response.GetDataLength()));
            await fileStream.FlushAsync();
            fileStream.Close();
        }
        catch (Exception)
        {
            target.Delete();
            return false;
        }
        
        return true;
    }

    public async Task<bool> TryLoadMods(FileInfo target)
    {
        ModsDownloadRequest request = new ModsDownloadRequest();
        ModsDownloadResponse response = (ModsDownloadResponse)await GetResponseFor(request);
        
        try
        {
            await using FileStream fileStream = target.Open(FileMode.OpenOrCreate);
            await fileStream.WriteAsync(response.ModsBytes.AsMemory(0, response.GetDataLength()));
            await fileStream.FlushAsync();
            fileStream.Close();
        }
        catch (Exception)
        {
            target.Delete();
            return false;
        }
        
        return true;
    }

    public async Task<bool> TryChangeSkin(User user, Skin skin)
    {
        byte[] skinData = skin.SkinBytes.ToArray();
        SkinChangeRequest request = new SkinChangeRequest(user.Nickname, skinData, skin.SkinBytes.Count);
        SkinChangeResponse response = (SkinChangeResponse)await GetResponseFor(request);
        return response.Success;
    }

    public async Task<Skin> GetSkin(User user)
    {
        GetSkinRequest request = new GetSkinRequest(user.Nickname);
        GetSkinResponse response = (GetSkinResponse)await GetResponseFor(request);
        return Skin.FromBytes(response.SkinData);
    }

    #endregion

    private bool ConnectToServer(string hostName, int port)
    {
        try
        {
            _client = new TcpClient(hostName, port);
            _networkStream = _client.GetStream();
            _networkChannel = new NetworkChannel(_networkStream);
            CancellationToken cancellationToken = _cancellationTokenSource.Token;
            Task.Run(
                () => SendRequestsWhenAvailable(_networkChannel, _requests, cancellationToken).GetAwaiter(),
                cancellationToken);
        }
        catch
        {
            return false;
        }

        return true;
    }

    private void Terminate()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _networkStream.Dispose();
        _client.Close();
    }

    private async Task SendRequestsWhenAvailable(NetworkChannel networkChannel, 
        ConcurrentQueue<Query> queryQueue, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (!queryQueue.TryDequeue(out Query query)) continue;
            await networkChannel.SendMessage(query.Request);
            MessageHeader header = await _networkChannel.ListenForHeader();
            Stream messageData = await _networkChannel.ListenForMessage(header);
            Message response = MessageRegistry.GetMessageFor(header);
            response.ApplyData(messageData);
            query.Response.SetResult(response);
        }
    }

    private Task<Message> GetResponseFor(Message request)
    {
        TaskCompletionSource<Message> taskCompletionSource = new TaskCompletionSource<Message>();
        _requests.Enqueue(new Query(request, taskCompletionSource));
        return taskCompletionSource.Task;
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