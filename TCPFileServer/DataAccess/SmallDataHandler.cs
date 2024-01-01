using Common.Models;

namespace TCPFileServer.DataAccess;

public sealed class SmallDataHandler: DataHandler
{
    public readonly string DefaultSkinPath;
    private readonly string _repositoryPath;
    private readonly string _usersDirectory;
    private readonly string _skinsDirectory;
    
    public SmallDataHandler(string repositoryPath)
    {
        _repositoryPath = repositoryPath;
        _usersDirectory = repositoryPath + @"Users/";
        _skinsDirectory = repositoryPath + @"Skins/";
        DefaultSkinPath = repositoryPath + @"DefaultSkin.png";
    }

    public bool UserExists(string username)
    {
        string filePath = _usersDirectory + username + ".xml";
        if (File.Exists(filePath)) return true;
        return false;
    }

    public User GetUser(string username)
    {
        string fileName = username + ".xml";
        using Stream fileStream = ReadFromRepository(_usersDirectory, fileName);
        User result = DataSerializer.UserFromXml(fileStream);
        if (result is null) throw new ArgumentOutOfRangeException(nameof(username), $"User [{username}] doesn't exist.");
        return result;
    }
    
    public LaunchConfiguration GetLaunchConfig()
    {
        string fileName = "LaunchConfiguration.xml";
        using Stream fileStream = ReadFromRepository(_repositoryPath, fileName);
        LaunchConfiguration launchConfig = DataSerializer.LaunchConfigFromXml(fileStream);
        return launchConfig;
    }
    
    public string[] GetAllNicknames()
    {
        DirectoryInfo usersDirectory = new DirectoryInfo(_usersDirectory);
        FileInfo[] files = usersDirectory.GetFiles("*.xml");
        
        string[] nicknames = new string[files.Length];
        for (int i = 0; i < nicknames.Length; i++)
        {
            FileInfo file = files[i];
            nicknames[i] = file.Name;
        }

        return nicknames;
    }

    public string SaveSkin(string nickname, byte[] data)
    {
        string skinPath = _skinsDirectory + nickname + ".png";
        if(File.Exists(skinPath))File.Delete(skinPath);
        using FileStream fileStream = new FileStream(skinPath, FileMode.Create);
        fileStream.Write(data, 0, data.Length);
        return skinPath;
    }
    
    public void RemoveSkin(string userSkinPath)
    {
        string path = _skinsDirectory + userSkinPath;
        if (!File.Exists(path)) throw new ArgumentException($"{userSkinPath} doesn't exists");
        File.Delete(path);
    }
    
    public Task RewriteUser(User newUser) 
    {
        string path = _usersDirectory + newUser.Nickname + ".xml";
        if (UserExists(newUser.Nickname)) File.Delete(path);
        CreateUser(newUser, path);
        return Task.CompletedTask;
    }
    
    private void CreateUser(User newUser, string path) 
    {
        using FileStream fileStream = new FileStream(path, FileMode.Create);
        DataSerializer.UserToXml(newUser, fileStream);
    }

    public void WriteLaunchConfig(LaunchConfiguration config)
    {
        FileInfo file = new FileInfo(_repositoryPath + "LaunchConfiguration.xml");
        if(file.Exists)file.Delete();
        using FileStream stream = file.Create();
        DataSerializer.LaunchConfigToXml(config, stream);
        stream.Flush();
    }
}