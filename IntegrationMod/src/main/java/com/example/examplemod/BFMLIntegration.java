package com.example.examplemod;

import com.example.examplemod.bfml.BFMLFileClient;
import com.mojang.logging.LogUtils;
import net.minecraftforge.common.MinecraftForge;
import net.minecraftforge.eventbus.api.SubscribeEvent;
import net.minecraftforge.fml.common.Mod;
import net.minecraftforge.fml.event.lifecycle.FMLCommonSetupEvent;
import net.minecraftforge.fml.javafmlmod.FMLJavaModLoadingContext;
import org.slf4j.Logger;

import java.io.IOException;

@Mod(BFMLIntegration.ID)
public class BFMLIntegration
{
    public static final String ID = "bfmlintegration";
    private static final Logger LOGGER = LogUtils.getLogger();
    private final ServerSkins serverSkins;
    private final BFMLFileClient bfmlFileClient;

    public BFMLIntegration()
    {
        FMLJavaModLoadingContext.get().getModEventBus().register(this);
        MinecraftForge.EVENT_BUS.register(this);

        bfmlFileClient = new BFMLFileClient();
        serverSkins = new ServerSkins(bfmlFileClient);
        MinecraftForge.EVENT_BUS.register(serverSkins);
    }

    @SubscribeEvent
    private void setup(FMLCommonSetupEvent event) throws IOException {
        //BFMLFileClient fileClient = new BFMLFileClient();v
        //fileClient.connectToServer();
    }
}
