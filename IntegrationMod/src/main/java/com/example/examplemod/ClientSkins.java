package com.example.examplemod;

import com.mojang.authlib.minecraft.MinecraftProfileTexture;
import net.minecraft.client.Minecraft;
import net.minecraft.client.multiplayer.PlayerInfo;
import net.minecraft.resources.ResourceLocation;
import net.minecraft.world.entity.player.Player;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.nio.ByteBuffer;
import java.nio.file.FileSystems;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.Map;

public class ClientSkins {
    public static void apply(ByteBuffer buffer, Player player){
        PlayerInfo playerInfo = Minecraft.getInstance().player.connection.getPlayerInfo(player.getUUID());
        ResourceLocation skin = save(buffer, player.getScoreboardName());
        rewriteRegistryEntry(skin, playerInfo);
    }

    private static ResourceLocation save(ByteBuffer buffer, String playerName){
        try {
            Path skinDirectory = getSkinsDirectory();
            File skinFile = new File(skinDirectory + "\\" + playerName + ".png");
            skinFile.createNewFile();
            try(FileOutputStream fileOutputStream = new FileOutputStream(skinFile)){
                fileOutputStream.write(buffer.array());
            }
            return new ResourceLocation(skinFile.getAbsolutePath());
        } catch (IOException e) {
            throw new RuntimeException(e);
        }
    }

    private static void rewriteRegistryEntry(ResourceLocation skin, PlayerInfo playerInfo){
        try {
            Map<MinecraftProfileTexture.Type, ResourceLocation> textureLocations =
                    (Map<MinecraftProfileTexture.Type, ResourceLocation>)
                            PlayerInfo.class
                                    .getDeclaredField("textureLocations")
                                    .get(playerInfo);

            textureLocations.put(MinecraftProfileTexture.Type.SKIN, skin);
        } catch (IllegalAccessException | NoSuchFieldException e) {
            throw new RuntimeException(e);
        }
    }

    private static Path getSkinsDirectory() throws IOException {

        ResourceLocation resourceLocation = new ResourceLocation(BFMLIntegration.ID, "playerskins");
        String minecraftPath = Minecraft.getInstance().gameDirectory.getAbsolutePath();
        Path skinDirectory = FileSystems.getDefault().getPath(
                minecraftPath.substring(0, minecraftPath.length()-2) +
                        "\\resources\\bfmlintegration\\playerskin");
        Files.createDirectories(skinDirectory);
        return skinDirectory;
    }
}