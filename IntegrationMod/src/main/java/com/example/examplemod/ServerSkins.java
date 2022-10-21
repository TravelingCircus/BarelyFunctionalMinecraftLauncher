package com.example.examplemod;

import com.example.examplemod.bfml.BFMLFileClient;
import net.minecraft.world.entity.player.Player;
import net.minecraftforge.event.entity.player.PlayerEvent;
import net.minecraftforge.eventbus.api.SubscribeEvent;
import org.apache.commons.lang3.NotImplementedException;

import java.nio.ByteBuffer;
import java.util.HashMap;

public class ServerSkins {
    private final BFMLFileClient fileClient;
    private final HashMap<String, ByteBuffer> skinsMap;

    public ServerSkins(BFMLFileClient fileClient) {
        this.fileClient = fileClient;
        this.skinsMap = new HashMap<>();
    }

    @SubscribeEvent
    public void onPlayerConnected(PlayerEvent.PlayerLoggedInEvent event){
        ByteBuffer skin = downloadSkin(event.getPlayer());
        skinsMap.put(event.getPlayer().getScoreboardName(), skin);
        notifyOneAboutAll(event.getPlayer(), skin);
        notifyAllAboutOne(event.getPlayer());
    }

    private ByteBuffer downloadSkin(Player player){
        return fileClient.downloadSkinForPlayer(player.getScoreboardName());
    }

    private void notifyAllAboutOne(Player one){
        throw new NotImplementedException();
    }

    private void notifyOneAboutAll(Player one, ByteBuffer skin){
        throw new NotImplementedException();
    }
}
