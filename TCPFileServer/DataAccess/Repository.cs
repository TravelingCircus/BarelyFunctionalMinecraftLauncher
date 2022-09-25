using CommonData;

namespace HTTPFileServer.DataAccess;

public sealed class Repository
{
    private readonly string _path;
    private readonly CancellationToken _cancellationToken;
    private readonly DataHandlerConsumersQueue<SmallDataHandler> _smallDataHandlerQueue;
    private readonly DataHandlerConsumersQueue<LargeDataHandler> _largeDataHandlerQueue;

    public Repository(string path, CancellationToken cancellationToken)
    {
        _path = path;
        _cancellationToken = cancellationToken;
        SmallDataHandler smallDataHandler = new SmallDataHandler();
        _smallDataHandlerQueue = new DataHandlerConsumersQueue<SmallDataHandler>(smallDataHandler);
        LargeDataHandler largeDataHandler = new LargeDataHandler();
        _largeDataHandlerQueue = new DataHandlerConsumersQueue<LargeDataHandler>(largeDataHandler);
    }

    public void Initialize()
    {
        Task.Run(()=>
        {
            _smallDataHandlerQueue.RunBlocking(_cancellationToken).GetAwaiter();
        }, _cancellationToken);
        Task.Run(()=>
        {
            _largeDataHandlerQueue.RunBlocking(_cancellationToken).GetAwaiter();
        }, _cancellationToken);
    }
    
    public async Task<User> GetUser(string name)
    {
        SmallDataHandler dataHandler = await _smallDataHandlerQueue.GetDataHandler();
        
        //TODO implement
        
        dataHandler.Release();
        throw new NotImplementedException();
    }
}