package com.example.examplemod.bfml;

import java.io.File;
import java.nio.file.NotDirectoryException;

public final class Checksum {

    public static long FromDirectory(File directory) throws NotDirectoryException {
        if (!directory.exists()) throw new NotDirectoryException("{directory}");
        File[] files = directory.listFiles();
        long[] hashes = new long[files.length];
        for (int i = 0; i < files.length; i++)
        {
            hashes[i] = files[i].length()/files.length;
        }

        int result = 53214;
        for (long hash : hashes) {
            result += hash;
        }
        return result;
    }
}
