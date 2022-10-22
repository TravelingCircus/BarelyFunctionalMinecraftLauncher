package com.example.examplemod.bfml;

import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.Socket;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;

public final class NetworkChannel{

    private final Socket socket;
    private final InputStream networkInputStream;
    private final OutputStream networkOutputStream;

    private NetworkChannel(Socket socket) throws IOException {
        this.socket = socket;
        this.networkInputStream = socket.getInputStream();
        this.networkOutputStream = socket.getOutputStream();
    }

    public static NetworkChannel start(String ip, int port) throws IOException {
        return new NetworkChannel(new Socket(ip, port));
    }

    public MessageHeader readHeader() throws IOException {
        byte[] byteHeader = new byte[5];
        networkInputStream.read(byteHeader, 0, 5);
        byte[] dataBytes = new byte[4];
        System.arraycopy(byteHeader, 1, dataBytes, 0, 4);
        ByteBuffer intBuffer = ByteBuffer.wrap(dataBytes);
        intBuffer.order(ByteOrder.LITTLE_ENDIAN);
        return new MessageHeader(byteHeader[0], intBuffer.getInt());
    }

    public ByteArrayInputStream readMessage(MessageHeader header) throws IOException {
        byte[] messageBytes = new byte[header.DataLength];
        networkInputStream.read(messageBytes, 0, header.DataLength);

        return new ByteArrayInputStream(messageBytes);
    }

    public void sendMessage(Message message) throws Exception {
        networkOutputStream.write(message.getMessageHeader().ToByteArray());
        message.writeDataTo(networkOutputStream);
        networkOutputStream.flush();
    }

    public void close(){
        try {
            socket.close();
        } catch (IOException e) {
            throw new RuntimeException(e);
        }
    }
}
