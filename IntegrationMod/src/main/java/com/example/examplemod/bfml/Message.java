package com.example.examplemod.bfml;

import org.apache.http.MethodNotSupportedException;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.nio.charset.StandardCharsets;

public abstract class Message {

    public Message() {

    }

    public abstract MessageHeader getMessageHeader() throws Exception;

    public abstract void applyData(InputStream stream) throws IOException, MethodNotSupportedException;

    public abstract byte[] getData() throws Exception;

    public void writeDataTo(OutputStream targetStream) throws Exception {
        targetStream.write(getData());
    }

    protected byte[] byteArrayReadStream(InputStream stream, int arrayLength) throws IOException {
        byte[] bytes = new byte[arrayLength];
        stream.read(bytes, 0, arrayLength);
        return bytes;
    }

    protected String stringReadStream(InputStream stream) throws IOException {
        int stringLength = intReadStream(stream);
        byte[] stringBuffer = new byte[stringLength];
        stream.read(stringBuffer, 0, stringLength);
        ByteBuffer buffer = ByteBuffer.wrap(stringBuffer);
        buffer.order(ByteOrder.LITTLE_ENDIAN);
        byte[] bufferBytes = buffer.array();
        String bufferString = new String(bufferBytes, StandardCharsets.UTF_8);
        return bufferString;
    }

    protected int intReadStream(InputStream stream) throws IOException {
        byte[] intBuffer = new byte[4];
        stream.read(intBuffer, 0, 4);
        ByteBuffer buffer = ByteBuffer.wrap(intBuffer);
        buffer.order(ByteOrder.LITTLE_ENDIAN);
        return buffer.getInt();
    }

    protected boolean boolReadStream(InputStream stream) throws IOException {
        byte[] byteBuffer = new byte[1];
        stream.read(byteBuffer, 0, 1);
        return (int) byteBuffer[0] != 0;
    }
}
