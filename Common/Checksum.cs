namespace Common;

public static class Checksum
{
    public static uint FromDirectory(DirectoryInfo directory)
    {
        if (!directory.Exists) throw new DirectoryNotFoundException($"{directory}");
        FileInfo[] files = directory.GetFiles();
        uint[] hashes = new uint[files.Length];
        for (int i = 0; i < files.Length; i++)
        {
            hashes[i] = unchecked((uint)files[i].Length/(uint)files.Length);
        }

        uint result = 53214;
        for (int i = 0; i < hashes.Length; i++)
        {
            result += hashes[i];
        }
        return result;
    }
}