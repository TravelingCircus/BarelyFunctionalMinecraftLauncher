package com.example.examplemod;

import com.example.examplemod.bfml.BFMLFileClient;
import com.example.examplemod.simplechannel.BFMLPacketHandler;
import com.example.examplemod.simplechannel.STCPlayerSkinMessage;
import net.minecraft.server.level.ServerPlayer;
import net.minecraft.world.entity.player.Player;
import net.minecraftforge.event.entity.player.PlayerEvent;
import net.minecraftforge.eventbus.api.SubscribeEvent;
import net.minecraftforge.network.NetworkDirection;
import net.minecraftforge.network.PacketDistributor;

import java.nio.ByteBuffer;
import java.util.HashMap;
import java.util.Map;
import java.util.UUID;

public class ServerSkins {
    private final BFMLFileClient fileClient;
    private final HashMap<UUID, ByteBuffer> skinsMap;

    public ServerSkins(BFMLFileClient fileClient) {
        this.fileClient = fileClient;
        this.skinsMap = new HashMap<>();
    }

    @SubscribeEvent
    public void onPlayerConnected(final PlayerEvent.PlayerLoggedInEvent event){
        fileClient.downloadSkinForPlayer(event.getPlayer().getScoreboardName(), (response)->{
            ByteBuffer skin = ByteBuffer.wrap(response.SkinData);
            skinsMap.put(event.getPlayer().getUUID(), skin);
            notifyOneAboutAll(event.getPlayer());
            notifyAllAboutOne(event.getPlayer(), skin);
        });
    }

    private void notifyAllAboutOne(Player one, ByteBuffer skin){
        BFMLPacketHandler.CHANNEL.send(PacketDistributor.ALL.noArg(), new STCPlayerSkinMessage(skin, one.getUUID()));
    }

    private void notifyOneAboutAll(Player one){
        for (Map.Entry<UUID, ByteBuffer> pair : skinsMap.entrySet()) {
            BFMLPacketHandler.CHANNEL.sendTo(
                    new STCPlayerSkinMessage(pair.getValue(), pair.getKey()),
                    ((ServerPlayer)one).connection.connection,
                    NetworkDirection.PLAY_TO_CLIENT);
        }
    }
}