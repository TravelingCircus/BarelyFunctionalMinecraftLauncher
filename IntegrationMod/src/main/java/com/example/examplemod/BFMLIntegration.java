package com.example.examplemod;

import com.example.examplemod.bfml.BFMLFileClient;
import com.example.examplemod.simplechannel.BFMLPacketHandler;
import com.mojang.logging.LogUtils;
import net.minecraftforge.common.MinecraftForge;
import net.minecraftforge.eventbus.api.SubscribeEvent;
import net.minecraftforge.fml.common.Mod;
import net.minecraftforge.fml.event.lifecycle.FMLCommonSetupEvent;
import net.minecraftforge.fml.javafmlmod.FMLJavaModLoadingContext;
import org.slf4j.Logger;

@Mod(BFMLIntegration.ID)
public class BFMLIntegration
{
    public static final String ID = "bfmlintegration";
    private static final Logger LOGGER = LogUtils.getLogger();
    private static ServerSkins serverSkins;
    private static BFMLFileClient bfmlFileClient;

    public BFMLIntegration()
    {
        FMLJavaModLoadingContext.get().getModEventBus().register(BFMLIntegration.class);
        MinecraftForge.EVENT_BUS.register(BFMLIntegration.class);
    }

    @SubscribeEvent
    public static void onCommonSetup(final FMLCommonSetupEvent event){
        BFMLPacketHandler.registerMessages();
        bfmlFileClient = new BFMLFileClient();
        bfmlFileClient.connectToServer();
        serverSkins = new ServerSkins(bfmlFileClient);
        MinecraftForge.EVENT_BUS.register(serverSkins);
    }
}
