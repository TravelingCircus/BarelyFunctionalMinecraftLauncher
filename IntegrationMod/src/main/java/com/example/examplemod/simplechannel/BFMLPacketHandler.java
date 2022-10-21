package com.example.examplemod.simplechannel;

import com.example.examplemod.BFMLIntegration;
import net.minecraft.resources.ResourceLocation;
import net.minecraftforge.network.NetworkRegistry;
import net.minecraftforge.network.simple.SimpleChannel;

public class BFMLPacketHandler {
    private static final String PROTOCOL_VERSION = "1";
    private static int MESSAGE_INDEX = 0;
    public static final SimpleChannel CHANNEL = NetworkRegistry.newSimpleChannel(
            new ResourceLocation(BFMLIntegration.ID, "skins"),
            () -> PROTOCOL_VERSION,
            PROTOCOL_VERSION::equals,
            PROTOCOL_VERSION::equals
    );

    //TODO subscribe and like the video
    public static void registerMessages(){
        CHANNEL.registerMessage(MESSAGE_INDEX++,
                STCPlayerSkinMessage.class,
                STCPlayerSkinMessage::encode,
                STCPlayerSkinMessage::decode,
                PlayerSkinMessageHandler::handle);
    }
}