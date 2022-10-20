package com.example.examplemod;

import org.apache.http.MethodNotSupportedException;

import java.io.IOException;
import java.lang.reflect.InvocationTargetException;
import java.util.function.Consumer;

public final class MessageListener implements Runnable{

    private final NetworkChannel networkChannel;
    private final Consumer<Message> onReceivedMessage;

    public MessageListener(NetworkChannel networkChannel, Consumer<Message> onReceivedMessage){
        this.networkChannel = networkChannel;
        this.onReceivedMessage = onReceivedMessage;
    }

    @Override
    public void run() {
        try {
            MessageHeader header = networkChannel.listenForHeader();

            Message message = MessageRegistry.getMessageFor(header);

            message.applyData(networkChannel.listenForMessage(header));

            onReceivedMessage.accept(message);

        } catch (IOException | InvocationTargetException | NoSuchMethodException |
                 InstantiationException | IllegalAccessException | MethodNotSupportedException e) {
            throw new RuntimeException(e);
        }
    }
}
