namespace CommonData.Network;

public abstract class MessageHandler
{
    public abstract bool CanHandle(Message message);
    public abstract Task Handle(Stream dataStream);
    public abstract Task<Message> GetResponse(Stream dataStream);

    public static Message GetRightTypeMessage(Message message)
    {
        throw new NotImplementedException();
        //Message rightMessage = _keyToMessage[message.GetHeader()[0]];
        //rightMessage = rightMessage.FromData(message.GetData());
    }
}