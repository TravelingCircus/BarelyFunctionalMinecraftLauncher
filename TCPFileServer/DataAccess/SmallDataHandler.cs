using CommonData;

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
        return UserDataSerializer.FromXML(ReadFromRepository(filePath, fileName));
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
}