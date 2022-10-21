package com.example.examplemod;

import com.example.examplemod.bfml.BFMLFileClient;
import net.minecraft.world.entity.player.Player;
import net.minecraftforge.event.entity.player.PlayerEvent;
import net.minecraftforge.eventbus.api.SubscribeEvent;

import java.nio.ByteBuffer;

public class ServerSkins {
    private final BFMLFileClient fileClient;

    public ServerSkins(BFMLFileClient fileClient) {
        this.fileClient = fileClient;
    }

    @SubscribeEvent
    public void onPlayerConnected(PlayerEvent.PlayerLoggedInEvent event){
        ByteBuffer skin = downloadSkin(event.getPlayer());
        notifyOneAboutAll(event.getPlayer());
        notifyAllAboutOne(event.getPlayer());
    }

    private ByteBuffer downloadSkin(Player player){
        return fileClient.downloadSkinForPlayer(player.getScoreboardName());
    }

    private void notifyAllAboutOne(Player one){

    }

    private void notifyOneAboutAll(Player one, ByteBuffer skin){

    }
}
