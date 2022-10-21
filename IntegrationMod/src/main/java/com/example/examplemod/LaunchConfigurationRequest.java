package com.example.examplemod;

import org.apache.http.MethodNotSupportedException;

import java.io.InputStream;

public final class LaunchConfigurationRequest extends Message {
    @Override
    public MessageHeader getMessageHeader() throws Exception {
        return new MessageHeader(MessageRegistry.getKeyForMessageType(LaunchConfigurationRequest.class), 0);
    }

    @Override
    public void applyData(InputStream stream) throws MethodNotSupportedException {
        throw new MethodNotSupportedException("That request don't support this method");
    }

    @Override
    public byte[] getData() {
        return new byte[0];
    }
}
