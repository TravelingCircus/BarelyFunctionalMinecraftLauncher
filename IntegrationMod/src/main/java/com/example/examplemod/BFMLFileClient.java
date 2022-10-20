package com.example.examplemod;

import com.mojang.logging.LogUtils;
import org.slf4j.Logger;

import java.io.*;
import java.net.Socket;

public final class BFMLFileClient {

    private static final Logger LOGGER = LogUtils.getLogger();

    private Socket clientSocket;
    private InputStream networkInputStream;
    private OutputStream networkOutputStream;
    private NetworkChannel networkChannel;

    public boolean connectToServer() throws IOException {
        try
        {
            clientSocket = new Socket("127.0.0.1", 69);
            LOGGER.info("Client connected");
            networkInputStream = clientSocket.getInputStream();
            networkOutputStream = clientSocket.getOutputStream();
            networkChannel = new NetworkChannel(networkInputStream, networkOutputStream);

            Thread messageListenerThread = new Thread(new MessageListener(networkChannel,
                    (message) -> {
                File skin = new File("D:\\Home\\Desktope\\TestDownloads\\javaSkin.png");
                        try {
                            skin.createNewFile();
                            try(FileOutputStream fileOutputStream = new FileOutputStream(skin)){
                                fileOutputStream.write(((GetSkinResponse)message).SkinData);
                            }
                        } catch (IOException e) {
                            throw new RuntimeException(e);
                        }
                    }));

            messageListenerThread.start();

            networkChannel.sendMessage(new GetSkinRequest("kok"));
        }
        catch (Exception e)
        {
            return false;
        }

        return true;
    }
}
