using System.Collections.Concurrent;
using System.Net.Sockets;
using CommonData;
using CommonData.Network;

namespace TCPFileClient;

public sealed class FileClient
{
    private TcpClient _client;
    private NetworkStream _networkStream;
    private NetworkChannel _networkChannel;
    private readonly ConcurrentQueue<Query> _requests;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public FileClient()
    {
        _requests = new ConcurrentQueue<Query>();
        _cancellationTokenSource = new CancellationTokenSource();
    }
    
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

    public Task<string> DownloadForgeFiles()
    {
        throw new NotImplementedException();
    }

    public Task<LaunchConfiguration> DownloadLaunchConfiguration()
    {
        throw new NotImplementedException();
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