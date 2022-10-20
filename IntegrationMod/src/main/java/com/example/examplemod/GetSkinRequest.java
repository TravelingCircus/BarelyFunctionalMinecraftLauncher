package com.example.examplemod;

import org.apache.http.MethodNotSupportedException;

import java.io.InputStream;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;

public final class GetSkinRequest extends Message{

    private String nickname;

    public GetSkinRequest(){

    }

    public GetSkinRequest(String nickname){
        this.nickname = nickname;
    }

    @Override
    public MessageHeader getMessageHeader() throws Exception {
        return new MessageHeader(MessageRegistry.getKeyForMessageType(GetSkinRequest.class),
                (Integer.SIZE / 8) + nickname.length());
    }

    @Override
    public void applyData(InputStream stream) throws MethodNotSupportedException {
        throw new MethodNotSupportedException("That request don't support this method");
    }

    @Override
    public byte[] getData() throws Exception {
        MessageHeader header = getMessageHeader();
        ByteBuffer byteBuffer = ByteBuffer.allocate(header.DataLength);
        byteBuffer.order(ByteOrder.LITTLE_ENDIAN);
        byteBuffer.putInt(nickname.length());
        byteBuffer.put(nickname.getBytes());
        return byteBuffer.array();
    }
}
