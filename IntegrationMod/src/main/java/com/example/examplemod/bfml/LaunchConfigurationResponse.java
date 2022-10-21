package com.example.examplemod.bfml;

import java.io.IOException;
import java.io.InputStream;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;

public final class LaunchConfigurationResponse extends Message {

    public String ModsChecksum;
    public String VanillaVersion;
    public String ForgeVersion;
    public int RequiredGriwnas;

    @Override
    public MessageHeader getMessageHeader() throws Exception {
        return new MessageHeader(MessageRegistry.getKeyForMessageType(LaunchConfigurationResponse.class),
                4 * (Integer.SIZE / 8) + VanillaVersion.length() + ForgeVersion.length() + ModsChecksum.length());
    }

    @Override
    public void applyData(InputStream stream) throws IOException {
        VanillaVersion = stringReadStream(stream);
        ForgeVersion= stringReadStream(stream);
        ModsChecksum = stringReadStream(stream);
        RequiredGriwnas = intReadStream(stream);
    }

    @Override
    public byte[] getData() throws Exception {
        MessageHeader header = getMessageHeader();
        ByteBuffer byteBuffer = ByteBuffer.allocate(header.DataLength);
        byteBuffer.order(ByteOrder.LITTLE_ENDIAN);
        byteBuffer.putInt(VanillaVersion.length());
        byteBuffer.put(VanillaVersion.getBytes());
        byteBuffer.putInt(ForgeVersion.length());
        byteBuffer.put(ForgeVersion.getBytes());
        byteBuffer.putInt(ModsChecksum.length());
        byteBuffer.put(ModsChecksum.getBytes());
        byteBuffer.putInt(RequiredGriwnas);

        return byteBuffer.array();
    }
}
