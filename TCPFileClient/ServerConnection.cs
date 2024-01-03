using System.Collections.Concurrent;
using System.Net.Sockets;
using Common;
using Common.Misc;
using Common.Models;
using Common.Network;
using Common.Network.Messages.ChangeSkin;
using Common.Network.Messages.ForgeDownload;
using Common.Network.Messages.GetSkin;
using Common.Network.Messages.Login;
using Common.Network.Messages.ModsDownload;
using Common.Network.Messages.Registration;
using Utils;

namespace FileClient;

public sealed class ServerConnection : IDisposable
{
    private TcpClient _client;
    private NetworkStream _networkStream;
    private NetworkChannel _networkChannel;
    private readonly ConcurrentQueue<Query> _requests;
    private readonly CancellationTokenSource _cancellationTokenSource;

    private ServerConnection()
    {
        _requests = new ConcurrentQueue<Query>();
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public static async Task<Result<ServerConnection>> Connect(string hostName, int port)
    {
        ServerConnection serverConnection = new ServerConnection();
        
        bool success = false;
        for (int i = 0; i < 5; i++)
        {
            success = serverConnection.TryConnectToServer(hostName, port);
            if(success) break;
            int delay = 200 * (int)Math.Pow(2, i);
            await Task.Delay(delay);
        }
        
        return success ? serverConnection : new Exception("Failed to connect to the server.");
    }

    public void Dispose()
    {
        Terminate();
    }

    #region Interface

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

    private bool TryConnectToServer(string hostName, int port)
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

    private Task<Message> GetResponseFor(Message request)
    {
        TaskCompletionSource<Message> taskCompletionSource = new TaskCompletionSource<Message>();
        _requests.Enqueue(new Query(request, taskCompletionSource));
        return taskCompletionSource.Task;
    }

    private static async Task SendRequestsWhenAvailable(NetworkChannel networkChannel, 
        ConcurrentQueue<Query> queryQueue, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (!queryQueue.TryDequeue(out Query query)) continue;
            await networkChannel.SendMessage(query.Request);
            MessageHeader header = await networkChannel.ListenForHeader();
            Stream messageData = await networkChannel.ListenForMessage(header);
            Message response = MessageRegistry.GetMessageFor(header);
            response.ApplyData(messageData);
            query.Response.SetResult(response);
        }
    }

    private class Query
    {
        internal readonly Message Request;
        internal readonly TaskCompletionSource<Message> Response;

        internal Query(Message request, TaskCompletionSource<Message> response)
        {
            Request = request;
            Response = response;
        }
    }
}