package com.example.examplemod.simplechannel;

import com.example.examplemod.ClientSkins;
import net.minecraft.client.Minecraft;
import net.minecraft.network.FriendlyByteBuf;
import net.minecraft.world.entity.player.Player;
import net.minecraftforge.network.NetworkEvent;

import java.nio.ByteBuffer;
import java.util.UUID;
import java.util.function.Supplier;

public class STCPlayerSkinMessage {
    private final ByteBuffer skinBuffer;
    private final UUID playerID;

    public STCPlayerSkinMessage(ByteBuffer skinBuffer, UUID playerID) {
        this.skinBuffer = skinBuffer;
        this.playerID = playerID;
    }

    public void encode(FriendlyByteBuf buffer){
        skinBuffer.rewind();
        buffer.writeInt(skinBuffer.remaining());
        buffer.writeBytes(skinBuffer);
        buffer.writeUUID(playerID);
    }

    public static STCPlayerSkinMessage decode(FriendlyByteBuf buffer){
        ByteBuffer downloadedSkinBuffer = ByteBuffer.allocate(buffer.readInt());
        buffer.readBytes(downloadedSkinBuffer);
        UUID playerID = buffer.readUUID();
        return new STCPlayerSkinMessage(downloadedSkinBuffer, playerID);
    }

    public void handle(Supplier<NetworkEvent.Context> ctx){
        Player player = Minecraft.getInstance().level.getPlayerByUUID(playerID);
        ClientSkins.apply(skinBuffer, player);
    }
}