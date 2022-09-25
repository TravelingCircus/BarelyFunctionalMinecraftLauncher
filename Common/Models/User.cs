namespace CommonData.Models;

[Serializable]
public class User
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
    public readonly string Nickname;
    public readonly string PasswordHash;
    private int _gryvnyasPaid;

    public User(string skinPath)
    {
        SkinPath = skinPath;
    }
    
    public User(string nickname, string passwordHash, int gryvnyasPaid, string skinPath)
    {
        Nickname = nickname;
        PasswordHash = passwordHash;
        _gryvnyasPaid = gryvnyasPaid;
        SkinPath = skinPath;
    }
}