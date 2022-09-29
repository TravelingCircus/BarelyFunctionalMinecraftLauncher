using CommonData.Models;

namespace CommonData.Network.Messages.Login;

public class LoginResponse : Message
{
    public bool Success;
    public User User;
    public byte[] SkinData;

    public LoginResponse()
    {
    }

    public LoginResponse(bool success, User user, byte[] skinData)
    {
        Success = success;
        User = user;
        SkinData = skinData;
    }

    public override MessageHeader GetHeader()
    {
        return new MessageHeader(MessageRegistry.GetKeyForMessageType(typeof(LoginResponse)), 
            1
            + User.Nickname.Length
            + User.PasswordHash.Length
            + 4 * sizeof(int)
            + SkinData.Length);
    }

    public override void ApplyData(Stream stream)
    {
        Success = BoolReadStream(stream);
        string nickname = StringReadStream(stream);
        string passwordHash = StringReadStream(stream);
        int grywniasPaid = IntReadStream(stream);
        int skinDataLength = IntReadStream(stream);
        _ = stream.Read(SkinData, 0, skinDataLength);
        User = new User(nickname, passwordHash, grywniasPaid);
    }

    protected override Stream GetData()
    {
        MemoryStream buffer = new MemoryStream(GetHeader().DataLength);
        WriteToStream(buffer, Success);
        WriteToStream(buffer, User.Nickname);
        WriteToStream(buffer, User.PasswordHash);
        WriteToStream(buffer, User.GryvnyasPaid);
        WriteToStream(buffer, SkinData.Length);
        buffer.Write(SkinData, 0, SkinData.Length);
        return buffer;
    }
}