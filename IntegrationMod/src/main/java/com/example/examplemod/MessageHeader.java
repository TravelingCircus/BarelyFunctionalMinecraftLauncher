package com.example.examplemod;

import java.nio.ByteBuffer;
import java.nio.ByteOrder;

public final class MessageHeader {

    public final byte MessageKey;
    public final int DataLength;

    public MessageHeader(byte messageKey, int dataLength)
    {
        MessageKey = messageKey;
        DataLength = dataLength;
    }

    public byte[] ToByteArray()
    {
        ByteBuffer buffer = ByteBuffer.allocate(5);
        buffer.order(ByteOrder.LITTLE_ENDIAN);
        buffer.put(MessageKey);
        buffer.putInt(DataLength);
        return buffer.array();
    }
}
