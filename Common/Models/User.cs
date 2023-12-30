namespace Common.Models;

[Serializable]
public sealed class User
{
    public string SkinPath;
    public string Nickname;
    public string PasswordHash;

    public User() : this("None", "None") { }

    public User(string nickname, string passwordHash, string skinPath = "")
    {
        Nickname = nickname;
        PasswordHash = passwordHash;
        SkinPath = skinPath;
    }
}