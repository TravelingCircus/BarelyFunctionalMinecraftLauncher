using System.Collections.Concurrent;
using System.Net.Sockets;
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

namespace TCPFileClient;

public sealed class FileClient
{
    private TcpClient _client;
    private NetworkStream _networkStream;
    private NetworkChannel _networkChannel;
    private readonly string _minecraftPath;
    private readonly ConcurrentQueue<Query> _requests;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public FileClient(string minecraftPath)
    {
        _minecraftPath = minecraftPath;
        _requests = new ConcurrentQueue<Query>();
        _cancellationTokenSource = new CancellationTokenSource();
    }

    #region Interface
    
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
    
    public async Task<ForgeDownloadResponse> DownloadForgeFiles(string tempDirectoryPath)
    {
        string tempForgePath = tempDirectoryPath + @"\Forge.zip";
        ForgeDownloadResponse response = (ForgeDownloadResponse)await GetResponseFor(new ForgeDownloadRequest());
        response.TempForgePath = tempForgePath;

        await using FileStream fileStream = new FileStream(response.TempForgePath, FileMode.OpenOrCreate);
        await fileStream.WriteAsync(response.ForgeBytes, 0, response.GetDataLength());
        await fileStream.FlushAsync();
        fileStream.Close();
        response.ForgeBytes = null!;
        
        return response;
    }
    
    public async Task<ModsDownloadResponse> DownloadMods(string directory)
    {
        ModsDownloadResponse response = (ModsDownloadResponse)await GetResponseFor(new ModsDownloadRequest());
        response.ModsZipPath = directory + @"\mods.zip";

        await using FileStream fileStream = new FileStream(response.ModsZipPath, FileMode.OpenOrCreate);
        await fileStream.WriteAsync(response.ModsBytes, 0, response.GetDataLength());
        await fileStream.FlushAsync();
        fileStream.Close();
        response.ModsBytes = null!;

        return response;
    }
    
    public async Task<GetSkinResponse> GetSkinFor(string nickname)
    {
        return (GetSkinResponse)await GetResponseFor(new GetSkinRequest(nickname));
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

    public void Disconnect()
    {
        Terminate();
    }
}