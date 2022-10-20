package com.example.examplemod;

import java.lang.reflect.InvocationTargetException;
import java.util.HashMap;
import java.util.Map;

public class MessageRegistry {
    private static final HashMap<Byte, Class> _messages = new HashMap<Byte, Class>(){
        {
            {put((byte)15, GetSkinRequest.class);}
        }
    };

    public static Message GetMessageFor(MessageHeader header) throws NoSuchMethodException,
            InvocationTargetException, InstantiationException, IllegalAccessException {
        return (Message) _messages.get(header.MessageKey).getDeclaredConstructor().newInstance();
    }

    public static String GetMessageTypeName(MessageHeader header)
    {
        return _messages.get(header.MessageKey).getTypeName();
    }

    public static byte GetKeyForMessageType(Class messageClass) throws Exception {
        String typeName = messageClass.getTypeName();
        for(Map.Entry<Byte, Class> entry: _messages.entrySet()){
            Class value = entry.getValue();
            if(value.getTypeName().equals(messageClass.getTypeName())) return entry.getKey();
        }

        throw new Exception("Argument index out of range");
    }
}
