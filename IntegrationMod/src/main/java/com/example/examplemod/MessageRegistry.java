package com.example.examplemod;

import java.lang.reflect.InvocationTargetException;
import java.util.HashMap;
import java.util.Map;

public class MessageRegistry {
    private static final HashMap<Byte, Class> messages = new HashMap<Byte, Class>(){
        {
            {put((byte)15, GetSkinRequest.class);}
            {put((byte)16, GetSkinResponse.class);}
        }
    };

    public static Message getMessageFor(MessageHeader header) throws NoSuchMethodException,
            InvocationTargetException, InstantiationException, IllegalAccessException {
        Class<GetSkinResponse> message = messages.get(header.MessageKey);
        return message.getDeclaredConstructor().newInstance();
    }

    public static String getMessageTypeName(MessageHeader header)
    {
        return messages.get(header.MessageKey).getTypeName();
    }

    public static byte getKeyForMessageType(Class messageClass) throws Exception {
        String typeName = messageClass.getTypeName();
        for(Map.Entry<Byte, Class> entry: messages.entrySet()){
            Class value = entry.getValue();
            if(value.getTypeName().equals(messageClass.getTypeName())) return entry.getKey();
        }

        throw new Exception("Argument index out of range");
    }
}
