namespace CommonData;

[Serializable]
public class User
{
    public string Nickname
    {
        get => _nickname;
        set => _nickname = value;
    }

    public string Password
    {
        get => _password;
        set => _password = value;
    }

    public int GryvnyasPaid
    {
        get => _gryvnyasPaid;
        set => _gryvnyasPaid = value;
    }

    private string _nickname;
    private string _password;
    private int _gryvnyasPaid;

    public User(string nickname, string password, int gryvnyasPaid)
    {
        _nickname = nickname;
        _password = password;
        _gryvnyasPaid = gryvnyasPaid;
    }
}