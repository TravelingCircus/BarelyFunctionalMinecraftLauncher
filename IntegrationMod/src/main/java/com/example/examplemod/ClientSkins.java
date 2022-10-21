package com.example.examplemod;

import net.minecraft.client.Minecraft;
import net.minecraft.client.multiplayer.PlayerInfo;
import net.minecraft.resources.ResourceLocation;
import net.minecraft.world.entity.player.Player;

import java.nio.ByteBuffer;
import java.util.UUID;

public class ClientSkins {
    public static void apply(ByteBuffer buffer, Player player){
        PlayerInfo playerInfo = Minecraft.getInstance().player.connection.getPlayerInfo(player.getUUID());
        ResourceLocation skin = save(buffer, player.getScoreboardName());
        rewriteRegistryEntry(skin, playerInfo);
    }

    private static ResourceLocation save(ByteBuffer buffer, String playerName){
        return null;
    }

    private static void rewriteRegistryEntry(ResourceLocation skin, PlayerInfo playerInfo){

    }
}
