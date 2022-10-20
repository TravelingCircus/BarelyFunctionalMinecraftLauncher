package com.example.examplemod;

import com.mojang.logging.LogUtils;
import org.slf4j.Logger;

import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.nio.ByteBuffer;
import java.util.stream.Stream;

public final class NetworkChannel {

    private static final Logger LOGGER = LogUtils.getLogger();

    private InputStream _networkInputStream;
    private OutputStream _networkOutputStream;

    public NetworkChannel(InputStream inputStream, OutputStream outputStream){
        this._networkInputStream = inputStream;
        this._networkOutputStream = outputStream;
    }

    public MessageHeader ListenForHeader() throws IOException {
        byte[] byteHeader = new byte[5];
        _networkInputStream.read(byteHeader, 0, 5);
        _networkInputStream.reset();
        byte[] dataBytes = new byte[4];
        for (int i = 1; i < 5; i++){
            dataBytes[i-1] = byteHeader[i];
        }
        return new MessageHeader(byteHeader[0], ByteBuffer.wrap(dataBytes).getInt());
    }

    public Stream ListenForMessage(MessageHeader header) throws IOException {
        byte[] messageBytes = _networkInputStream.readAllBytes();
        _networkInputStream.reset();
        InputStream inputStream = new ByteArrayInputStream(messageBytes);

        return (Stream) inputStream;
    }

    public void SendMessage(Message message) throws Exception {
        _networkOutputStream.write(message.getMessageHeader().ToByteArray());
        message.writeDataTo(_networkOutputStream);
        _networkOutputStream.flush();
        LOGGER.info("Sent message");
    }
}
