package com.example.examplemod;

import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.util.stream.*;

public final class GetSkinRequest extends Message{

    private String _nickname;

    public GetSkinRequest(String nickname){
        this._nickname = nickname;
    }

    @Override
    public MessageHeader getMessageHeader() throws Exception {
        return new MessageHeader(MessageRegistry.GetKeyForMessageType(GetSkinRequest.class), (Integer.SIZE / 8) + _nickname.length());
    }

    @Override
    public void ApplyData(Stream stream) {

    }

    @Override
    public byte[] getData() throws Exception {
        MessageHeader header = getMessageHeader();
        ByteBuffer byteBuffer = ByteBuffer.allocate(header.DataLength);
        byteBuffer.order(ByteOrder.LITTLE_ENDIAN);
        byteBuffer.putInt(_nickname.length());
        byteBuffer.put(_nickname.getBytes());
        return byteBuffer.array();
    }
}
