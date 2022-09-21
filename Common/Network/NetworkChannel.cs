namespace CommonData.Network;

public class NetworkChannel
{
    public Task SendMessage(Message message)
    {
        throw new NotImplementedException();
    }

    public Task<Message> ListenForMessage()
    {
        throw new NotImplementedException();
    }

    private void PickHandler()
    {
        throw new NotImplementedException();
    }
}