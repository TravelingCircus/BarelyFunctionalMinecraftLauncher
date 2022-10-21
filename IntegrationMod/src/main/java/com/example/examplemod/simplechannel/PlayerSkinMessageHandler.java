package com.example.examplemod.simplechannel;

import net.minecraftforge.api.distmarker.Dist;
import net.minecraftforge.fml.DistExecutor;
import net.minecraftforge.network.NetworkEvent;

import java.util.function.Supplier;

public class PlayerSkinMessageHandler {
    public static void handle(STCPlayerSkinMessage message, Supplier<NetworkEvent.Context> ctx){
        ctx.get().enqueueWork(()->{
            DistExecutor.unsafeRunWhenOn(Dist.CLIENT, ()-> ()-> {
                message.handle(ctx);
            });
        });
        ctx.get().setPacketHandled(true);
    }
}
