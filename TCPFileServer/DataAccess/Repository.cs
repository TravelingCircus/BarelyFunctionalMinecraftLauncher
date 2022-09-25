using CommonData.Models;

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
        SmallDataHandler smallDataHandler = new SmallDataHandler(_path);
        _smallDataHandlerQueue = new DataHandlerConsumersQueue<SmallDataHandler>(smallDataHandler);
        LargeDataHandler largeDataHandler = new LargeDataHandler(_path);
        _largeDataHandlerQueue = new DataHandlerConsumersQueue<LargeDataHandler>(largeDataHandler);
    }

    public void Initialize()
    {
        Task.Run(() =>
        {
            _smallDataHandlerQueue.RunBlocking(_cancellationToken).GetAwaiter();
        }, _cancellationToken);
        Task.Run(() =>
        {
            _largeDataHandlerQueue.RunBlocking(_cancellationToken).GetAwaiter();
        }, _cancellationToken);
    }

    #region Interface

    public async Task<User> GetUser(string name)
    {
        SmallDataHandler dataHandler = await _smallDataHandlerQueue.GetDataHandler();

        User user = dataHandler.GetUser(name);

        dataHandler.Release();
        return user;
    }

    public async Task<byte[]> GetSkin(string name)
    {
        SmallDataHandler dataHandler = await _smallDataHandlerQueue.GetDataHandler();

        byte[] skin = GetSkin(name, dataHandler);
        
        dataHandler.Release();
        return skin;
    }

    public async Task<(string name, byte[] skin)[]> GetAllSkins()
    {
        SmallDataHandler dataHandler = await _smallDataHandlerQueue.GetDataHandler();

        string[] nicknames = dataHandler.GetAllNicknames();
        (string name, byte[] skin)[] result = new (string name, byte[] skin)[nicknames.Length];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = (nicknames[i], GetSkin(nicknames[i], dataHandler));
        }
        
        dataHandler.Release();
        return result;
    }

    public async Task UpdateUserSkin(string name, byte[] skin)
    {
        SmallDataHandler dataHandler = await _smallDataHandlerQueue.GetDataHandler();

        User user = dataHandler.GetUser(name);
        string newSkinPath = dataHandler.SaveSkin(skin);
        user.SkinPath = newSkinPath;
        await dataHandler.RewriteUser(user);
        
        dataHandler.Release();
    }

    public async Task<bool> TryRegisterUser(User newUser)
    {
        SmallDataHandler dataHandler = await _smallDataHandlerQueue.GetDataHandler();

        if (dataHandler.UserExists(newUser.Nickname))
        {
            dataHandler.Release();
            return false;
        }

        await dataHandler.RewriteUser(newUser);
        
        dataHandler.Release();
        return true;
    }

    public async Task<bool> TryLogIn(string name, string enteredHash)
    {
        SmallDataHandler dataHandler = await _smallDataHandlerQueue.GetDataHandler();

        User user = dataHandler.GetUser(name);

        dataHandler.Release();
        return user.PasswordHash == enteredHash;
    }
    
    private byte[] GetSkin(string name, SmallDataHandler dataHandler)
    {
        User user = dataHandler.GetUser(name);
        return dataHandler.ReadFromRepository(user.SkinPath);
    }
    
    #endregion
}