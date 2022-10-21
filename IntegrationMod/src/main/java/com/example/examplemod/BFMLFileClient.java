package com.example.examplemod;

import com.mojang.logging.LogUtils;
import net.minecraft.client.Minecraft;
import org.apache.commons.lang3.NotImplementedException;
import org.slf4j.Logger;

import java.io.*;
import java.net.Socket;
import java.nio.file.NotDirectoryException;

public final class BFMLFileClient {

    private static final Logger LOGGER = LogUtils.getLogger();

    private Socket clientSocket;
    private InputStream networkInputStream;
    private OutputStream networkOutputStream;
    private NetworkChannel networkChannel;

    public boolean connectToServer(){
        try
        {
            clientSocket = new Socket("127.0.0.1", 69);
            networkInputStream = clientSocket.getInputStream();
            networkOutputStream = clientSocket.getOutputStream();
            networkChannel = new NetworkChannel(networkInputStream, networkOutputStream);

            Thread messageListenerThread = new Thread(new MessageListener(networkChannel,
                    (message) -> {
                    if(message instanceof GetSkinResponse){
                        createSkinFrom(message);
                    } else if (message instanceof LaunchConfigurationResponse) {
                         checkModsFrom(message);
                    }
            }));

            messageListenerThread.start();

            networkChannel.sendMessage(new LaunchConfigurationRequest());
        }
        catch (Exception e)
        {
            return false;
        }
        return true;
    }

    private void createSkinFrom(Message message){
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

    private void checkModsFrom(Message message){
        try{
            long requiredModsChecksum = Checksum.FromDirectory(new File(Minecraft.getInstance().gameDirectory + "\\mods"));
            long modsChecksum = Long.parseLong(((LaunchConfigurationResponse)message).ModsChecksum);
            if(modsChecksum == requiredModsChecksum) {
                LOGGER.info("Nice cock(mods)");
                return;
            }
            throw new NotImplementedException(); //TODO crash minecraft or show error
        }
        catch (NotDirectoryException e){
            throw new RuntimeException(e);
        }
    }
}
