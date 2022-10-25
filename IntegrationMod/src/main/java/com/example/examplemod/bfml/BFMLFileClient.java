package com.example.examplemod.bfml;

import com.mojang.logging.LogUtils;
import net.minecraft.client.Minecraft;
import net.minecraft.util.Tuple;
import org.apache.commons.lang3.NotImplementedException;
import org.slf4j.Logger;

import java.io.*;
import java.net.Socket;
import java.nio.ByteBuffer;
import java.nio.file.NotDirectoryException;
import java.util.LinkedList;
import java.util.Queue;
import java.util.concurrent.BlockingQueue;
import java.util.concurrent.LinkedBlockingQueue;
import java.util.function.Consumer;

public final class BFMLFileClient {

    private static final Logger LOGGER = LogUtils.getLogger();
    private NetworkChannel networkChannel;
    private BlockingQueue<Tuple<Message, Consumer<Message>>> requestQueue;
    private MessagingLoop responseListenThread;
    private Thread messageListenerThread;

    public BFMLFileClient() {
        this.requestQueue = new LinkedBlockingQueue<>();
    }

    public boolean connectToServer(){
        try
        {
            networkChannel = NetworkChannel.start("127.0.0.1", 69);
            responseListenThread = new MessagingLoop(networkChannel, requestQueue);

            messageListenerThread = new Thread(responseListenThread);
            messageListenerThread.start();
        }
        catch (Exception e)
        {
            return false;
        }
        return true;
    }

    public void sendMessage(Message message, Consumer<Message> onResponse){
        requestQueue.offer(new Tuple<>(message, onResponse));
        //messageListenerThread.notify();
    }

    public void downloadSkinForPlayer(String playerName, Consumer<GetSkinResponse> onDownloaded){
        sendMessage(new GetSkinRequest(playerName), (message)->{
            onDownloaded.accept((GetSkinResponse) message);
        });
    }

    private void createSkinMessageFrom(Message message){
        File skin = new File("D:\\Home\\Desktope\\TestDownloads\\javaSkin.png");
        try {
            skin.createNewFile();
            try(FileOutputStream fileOutputStream = new FileOutputStream(skin)){
                fileOutputStream.write(((GetSkinResponse)message).SkinData);
            }
        } catch (IOException e) {
            throw new RuntimeException(e);
        }
    }

    private void checkModsMessageFrom(Message message){
        try{
            long requiredModsChecksum = Checksum.FromDirectory(new File(Minecraft.getInstance().gameDirectory + "\\mods"));
            long modsChecksum = Long.parseLong(((LaunchConfigurationResponse)message).ModsChecksum);
            if(modsChecksum == requiredModsChecksum) {
                LOGGER.info("Nice cock(mods)");
                return;
            }
            throw new NotImplementedException();
        }
        catch (NotDirectoryException e){
            throw new RuntimeException(e);
        }
    }

    public void disconnect(){
        networkChannel.close();
        responseListenThread.stop();
    }
}