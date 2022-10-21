package com.example.examplemod;

import com.mojang.logging.LogUtils;
import net.minecraftforge.common.MinecraftForge;
import net.minecraftforge.fml.common.Mod;
import net.minecraftforge.fml.event.lifecycle.FMLCommonSetupEvent;
import net.minecraftforge.fml.javafmlmod.FMLJavaModLoadingContext;
import org.slf4j.Logger;

import java.io.IOException;

@Mod("bfmlintegration")
public class BFMLIntegration
{
    private static final Logger LOGGER = LogUtils.getLogger();

    public BFMLIntegration()
    {
        FMLJavaModLoadingContext.get().getModEventBus().addListener((FMLCommonSetupEvent event) -> {
            try {
                setup(event);
            } catch (IOException e) {
                throw new RuntimeException(e);
            }
        });

        MinecraftForge.EVENT_BUS.register(this);
    }

    private void setup(final FMLCommonSetupEvent event) throws IOException {
        BFMLFileClient fileClient = new BFMLFileClient();
        fileClient.connectToServer();
    }
}
