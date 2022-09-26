namespace CommonData.Models;

[Serializable]
public sealed class User
{

    public int GryvnyasPaid
    {
        get => _gryvnyasPaid;
        set
        {
            if (value < _gryvnyasPaid) throw new ArgumentException("Can't subtract money paid by user");
            _gryvnyasPaid = value;
        }
    }

    public string SkinPath;
    public string Nickname;
    public string PasswordHash;
    private int _gryvnyasPaid;

    public User()
    {
        
    }

    public User(string nickname, string passwordHash, int gryvnyasPaid = 0, string skinPath = "")
    {
        Nickname = nickname;
        PasswordHash = passwordHash;
        _gryvnyasPaid = gryvnyasPaid;
        SkinPath = skinPath;
    }
}