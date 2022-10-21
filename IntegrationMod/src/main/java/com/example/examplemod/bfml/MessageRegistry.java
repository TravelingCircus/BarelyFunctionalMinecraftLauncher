package com.example.examplemod.bfml;

import java.lang.reflect.InvocationTargetException;
import java.util.HashMap;
import java.util.Map;

public final class MessageRegistry {
    private static final HashMap<Byte, Class> messages = new HashMap<Byte, Class>(){
        {
            {put((byte)5, LaunchConfigurationRequest.class);}
            {put((byte)6, LaunchConfigurationResponse.class);}
            {put((byte)15, GetSkinRequest.class);}
            {put((byte)16, GetSkinResponse.class);}
        }
    };

    public static Message getMessageFor(MessageHeader header) throws NoSuchMethodException,
            InvocationTargetException, InstantiationException, IllegalAccessException {
        Class<Message> message = messages.get(header.MessageKey);
        return message.getDeclaredConstructor().newInstance();
    }

    public static String getMessageTypeName(MessageHeader header)
    {
        return messages.get(header.MessageKey).getTypeName();
    }

    public static byte getKeyForMessageType(Class messageClass) throws Exception {
        for(Map.Entry<Byte, Class> entry: messages.entrySet()){
            Class value = entry.getValue();
            if(value.getTypeName().equals(messageClass.getTypeName())) return entry.getKey();
        }

        throw new Exception("Argument index out of range");
    }
}
