package com.example.examplemod.simplechannel;

import net.minecraft.network.FriendlyByteBuf;
import net.minecraftforge.network.NetworkEvent;

import java.nio.ByteBuffer;
import java.util.function.Supplier;

public class STCPlayerSkinMessage {
    private final ByteBuffer skinBuffer;

    public STCPlayerSkinMessage(ByteBuffer skinBuffer) {
        this.skinBuffer = skinBuffer;
    }

    public void encode(FriendlyByteBuf buffer){
        skinBuffer.rewind();
        buffer.writeInt(skinBuffer.remaining());
        buffer.writeBytes(skinBuffer);
    }

    public static STCPlayerSkinMessage decode(FriendlyByteBuf buffer){
        return null;
        //ByteBuffer
    }

    public void handle(Supplier<NetworkEvent.Context> ctx){

    }
}