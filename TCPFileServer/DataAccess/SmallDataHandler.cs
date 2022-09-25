using CommonData;
using CommonData.Models;

namespace HTTPFileServer.DataAccess;

public class SmallDataHandler: DataHandler
{
    private readonly string _repositoryPath;
    
    public SmallDataHandler(string repositoryPath)
    {
        _repositoryPath = repositoryPath;
    }

    public bool CheckUser(string fileName)
    {
        string filePath = _repositoryPath + fileName;
        if (File.Exists(filePath)) return true;
        return false;
    }

    public User GetUser(string username)
    {
        string fileName = username + ".xml";
        string filePath = _repositoryPath + fileName;
        User result = UserDataSerializer.FromXML(ReadFromRepository(filePath, fileName));
        if (result is null) throw new ArgumentOutOfRangeException(nameof(username), $"User [{username}] doesn't exist.");
        return result;
    }
    
    public LaunchConfiguration GetLaunchConfig()
    {
        string fileName = "LaunchConfiguration.xml";
        string filePath = _repositoryPath + fileName;
        //TODO LaunchConfig FromXML(ReadFromRepository(filePath, fileName));
        throw new NotImplementedException();
    }
    
    //TODO Version class and GetVersion();

    public override Task WriteToRepository()
    {
        throw new NotImplementedException();
    }

    public string[] GetAllNicknames()
    {
        throw new NotImplementedException();
    }
    
    public string SaveSkin(byte[] data)
    {
        throw new NotImplementedException();
    }

    public bool UserExists(string name)
    {
        throw new NotImplementedException();
    }

    public Task RewriteUser(User newUser)
    {
        throw new NotImplementedException();
    }

    public void RemoveSkin(string userSkinPath)
    {
        throw new NotImplementedException();
    }
}