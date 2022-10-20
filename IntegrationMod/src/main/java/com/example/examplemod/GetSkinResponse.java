package com.example.examplemod;

import java.io.IOException;
import java.io.InputStream;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;

public final class GetSkinResponse extends Message{

    public int SkinDataLength;
    public byte[] SkinData;

    public GetSkinResponse() {

    }

    public GetSkinResponse(int skinDataLength, byte[] skinData)
    {
        SkinDataLength = skinDataLength;
        SkinData = skinData;
    }

    @Override
    public MessageHeader getMessageHeader() throws Exception {
        return new MessageHeader(MessageRegistry.getKeyForMessageType(GetSkinResponse.class),
                (Integer.SIZE / 8) + SkinDataLength);
    }

    @Override
    public void applyData(InputStream stream) throws IOException {
        SkinDataLength = intReadStream(stream);
        SkinData = byteArrayReadStream(stream, SkinDataLength);
    }

    @Override
    public byte[] getData() throws Exception {
        MessageHeader header = getMessageHeader();
        ByteBuffer byteBuffer = ByteBuffer.allocate(header.DataLength);
        byteBuffer.order(ByteOrder.LITTLE_ENDIAN);
        byteBuffer.putInt(SkinDataLength);
        byteBuffer.put(SkinData);
        return byteBuffer.array();
    }
}
