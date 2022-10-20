package com.example.examplemod;

import com.mojang.logging.LogUtils;
import org.slf4j.Logger;

import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;

public final class NetworkChannel{

    private static final Logger LOGGER = LogUtils.getLogger();

    private final InputStream networkInputStream;
    private final OutputStream networkOutputStream;

    public NetworkChannel(InputStream inputStream, OutputStream outputStream){
        this.networkInputStream = inputStream;
        this.networkOutputStream = outputStream;
    }

    public MessageHeader listenForHeader() throws IOException {
        byte[] byteHeader = new byte[5];
        networkInputStream.read(byteHeader, 0, 5);
        byte[] dataBytes = new byte[4];
        System.arraycopy(byteHeader, 1, dataBytes, 0, 4);
        ByteBuffer intBuffer = ByteBuffer.wrap(dataBytes);
        intBuffer.order(ByteOrder.LITTLE_ENDIAN);
        return new MessageHeader(byteHeader[0], intBuffer.getInt());
    }

    public ByteArrayInputStream listenForMessage(MessageHeader header) throws IOException {
        byte[] messageBytes = new byte[header.DataLength];
        networkInputStream.read(messageBytes, 0, header.DataLength);

        return new ByteArrayInputStream(messageBytes);
    }

    public void sendMessage(Message message) throws Exception {
        networkOutputStream.write(message.getMessageHeader().ToByteArray());
        message.writeDataTo(networkOutputStream);
        networkOutputStream.flush();
        LOGGER.info("Sent message");
    }
}
