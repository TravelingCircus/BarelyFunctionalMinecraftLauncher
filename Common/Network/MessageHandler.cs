namespace CommonData.Network;

public abstract class MessageHandler
{
    /*private static Dictionary<byte, Message> _keyToMessage = new Dictionary<byte, Message>()
    {
        {0, new Message()}
    };*/
    
    public abstract bool CanHandle(Message message);
    public abstract Task Handle(Message message);
    public abstract Task<Message> GetResponse(Message message);

    public static Message GetRightTypeMessage(Message message)
    {
        throw new NotImplementedException();
        //Message rightMessage = _keyToMessage[message.GetHeader()[0]];
        //rightMessage = rightMessage.FromData(message.GetData());
    }
}