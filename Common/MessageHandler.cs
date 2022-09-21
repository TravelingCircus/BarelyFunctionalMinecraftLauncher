namespace CommonData;

public abstract class MessageHandler
{
    public abstract bool CanHandle(Message message);
    public abstract void Handle(Message message);
    public abstract Message GetResponse(Message message);
}