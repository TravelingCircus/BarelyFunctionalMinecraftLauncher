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
import java.util.function.Consumer;

public final class BFMLFileClient {

    private static final Logger LOGGER = LogUtils.getLogger();

    private Socket clientSocket;
    private InputStream networkInputStream;
    private OutputStream networkOutputStream;
    private NetworkChannel networkChannel;
    private Queue<Tuple<Message, Consumer<Message>>> requestQueue;
    private MessagingLoop responseListenThread;

    public BFMLFileClient() {
        this.requestQueue = new LinkedList<>();
    }

    public boolean connectToServer(){
        try
        {
            networkChannel = NetworkChannel.start("127.0.0.1", 69);
            responseListenThread = new MessagingLoop(networkChannel, requestQueue);

            Thread messageListenerThread = new Thread(responseListenThread);
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
    }

    public ByteBuffer downloadSkinForPlayer(String playerName){
        throw new NotImplementedException();
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