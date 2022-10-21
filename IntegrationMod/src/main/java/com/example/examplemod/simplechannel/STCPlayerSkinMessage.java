package com.example.examplemod.simplechannel;

import net.minecraft.network.FriendlyByteBuf;
import net.minecraftforge.network.NetworkEvent;

import java.nio.ByteBuffer;
import java.util.function.Supplier;

public class STCPlayerSkinMessage {
    private ByteBuffer skinBuffer;

    public void encode(FriendlyByteBuf buffer){

    }

    public static STCPlayerSkinMessage decode(FriendlyByteBuf buffer){
        return null;
    }

    public void handle(Supplier<NetworkEvent.Context> ctx){

    }
}