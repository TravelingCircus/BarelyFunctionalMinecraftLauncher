package com.example.examplemod.bfml;

import net.minecraft.util.Tuple;
import org.apache.http.MethodNotSupportedException;

import java.io.IOException;
import java.lang.reflect.InvocationTargetException;
import java.util.Queue;
import java.util.function.Consumer;

public final class MessagingLoop implements Runnable{

    private final NetworkChannel networkChannel;
    private final Queue<Tuple<Message, Consumer<Message>>> requestQueue;
    private boolean stopped;

    public MessagingLoop(NetworkChannel networkChannel, Queue<Tuple<Message, Consumer<Message>>> requestQueue){
        this.networkChannel = networkChannel;
        this.requestQueue = requestQueue;
    }

    @Override
    public void run() {
        while (!stopped){
            if(requestQueue.isEmpty()) continue;
            Tuple<Message, Consumer<Message>> request = requestQueue.poll();

            try{
                networkChannel.sendMessage(request.getA());
                MessageHeader header = networkChannel.readHeader();
                Message message = MessageRegistry.getMessageFor(header);
                message.applyData(networkChannel.readMessage(header));
                request.getB().accept(message);
            } catch (Exception e) {
                throw new RuntimeException(e);
            }
        }
    }

    public void stop(){
        stopped = true;
    }
}
