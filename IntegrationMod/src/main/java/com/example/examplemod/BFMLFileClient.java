package com.example.examplemod;

import com.mojang.logging.LogUtils;
import org.slf4j.Logger;

import java.io.*;
import java.net.Socket;

public final class BFMLFileClient {

    private static final Logger LOGGER = LogUtils.getLogger();

    private Socket _clientSocket;
    private InputStream _networkInputStream;
    private OutputStream _networkOutputStream;
    private NetworkChannel _networkChannel;

    public boolean ConnectToServer() throws IOException {
        try
        {
            _clientSocket = new Socket("127.0.0.1", 69);
            LOGGER.info("Client connected");
            _networkInputStream = _clientSocket.getInputStream();
            _networkOutputStream = _clientSocket.getOutputStream();
            _networkChannel = new NetworkChannel(_networkInputStream, _networkOutputStream);

            _networkChannel.SendMessage(new GetSkinRequest("kok"));
        }
        catch (Exception e)
        {
            return false;
        }

        return true;
    }
}
