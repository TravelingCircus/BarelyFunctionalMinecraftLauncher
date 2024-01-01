namespace Common.Models;

[Serializable]
public sealed class User
{
    public Guid Guid { get; private set; }
    public readonly string Nickname;
    public readonly string PasswordHash;

    public User() : this("None", "None") { }

    public User(string nickname, string passwordHash)
    {
        Nickname = nickname;
        PasswordHash = passwordHash;
        Guid = Guid.Empty;
    }
}