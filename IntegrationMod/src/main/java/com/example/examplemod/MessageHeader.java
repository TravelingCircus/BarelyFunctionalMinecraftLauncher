package com.example.examplemod;

import com.mojang.logging.LogUtils;
import org.slf4j.Logger;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;

public final class MessageHeader {

    private static final Logger LOGGER = LogUtils.getLogger();

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
