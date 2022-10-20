package com.example.examplemod;

import com.mojang.logging.LogUtils;
import org.slf4j.Logger;

import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.io.OutputStream;
import java.util.stream.Stream;

public abstract class Message {

    private static final Logger LOGGER = LogUtils.getLogger();

    public abstract MessageHeader getMessageHeader() throws Exception;

    public abstract void ApplyData(Stream stream);

    public abstract byte[] getData() throws Exception;

    public void writeDataTo(OutputStream targetStream) throws Exception {
        targetStream.write(getData());
        LOGGER.info("Written data to stream");
    }
}
