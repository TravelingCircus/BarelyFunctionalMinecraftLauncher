using CommonData;

namespace HTTPFileServer.DataAccess;

public class Repository
{
    private string _path;

    public Repository(string path)
    {
        _path = path;
    }

    public Task<bool> AddNewUser(User user)
    {
        string filePath = _path + $"/{user.Nickname}.xml";
        if (File.Exists(filePath)) return Task.FromResult(false);

        using (FileStream fileStream = File.Create(filePath))
        {
            UserDataSerializer.ToXML(user, fileStream);
        }
        
        return Task.FromResult(true);
    }
}